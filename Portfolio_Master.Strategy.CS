using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public static class PortfolioDataKeys
    {
        public const string CRTDR = "CRTDR";
        public const string MoneyManagementCommand = "MoneyManagementCommand";
        public const string IWantToBuy = "IWantToBuy";
        public const string IWantToSell = "IWantToSell";
        public const string NumberOfShares = "NumberOfShares";
    }

    public enum StrategyEvents         // The list of possible messages, sent between the strategy and MM-signal
    {
        None = 0,                      // nothing
        GenerateOrders_Long,           // Order generation command
        GenerateOrders_Short,          // Sell generation command
    }

    public class Portfolio_Master : PortfolioSignalObject
    {
        [Input]
        public double SkalpingFactor { get; set; }

        [Input]
        public string TriggerTime { get; set; }
        TimeSpan triggerTime;

        private K_RatioMath m_KRatio;

        private DateTime m_LastTraceTime = DateTime.MinValue;

        public Portfolio_Master(object _ctx)
            : base(_ctx)
        {
            SkalpingFactor = 1.1;
            TriggerTime = "15:58:30";
            DoReinvestment = true;
        }

        protected override void Create()
        {
            m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
            triggerTime = TimeSpan.Parse(TriggerTime);

            if (Environment.ApplicationCode != EApplicationCode.Portfolio)
            {
                ExecControl.Abort("Signal can be applied in portfolio only.");
            }

            m_KRatio.NumberOfDataBarsInCalendarYear = 1; //252;
        }

        protected override void StopCalc()
        {
            if (!Environment.Optimizing)
            {
                Output.WriteLine("Dumping Money Management Signal log");
                DumpLog(false);
            }
        }

        [Input]
        public bool DoReinvestment { get; set; }

        private readonly Dictionary<string, DateTime> logMessages = new Dictionary<string, DateTime>();

        private void Log(string message, params object[] parameters)
        {
            Log(false, message, parameters);
        }

        private void LogAndMail(string message, params object[] parameters)
        {
            Log(true, message, parameters);
        }

        private void Log(bool sendMail, string message, params object[] parameters)
        {
            if (!Environment.Optimizing)
            {
                if (Environment.IsRealTimeCalc)
                {
                    try
                    {
                        var str = string.Format("{0}: {1}", Bars.TimeValue, string.Format(message, parameters));

                        Output.WriteLine(string.Format("{0} - {1}", DateTime.Now, str));

                        logMessages[str] = DateTime.Now;

                        if (sendMail)
                        {
                            DumpLog(true);
                        }
                        //                    else if(logMessages.Count >= 1000)
                        //                    {
                        //                        DumpLog(false);
                        //                    }
                    }
                    catch (Exception e)
                    {
                        Output.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void DumpLog(bool sendMail)
        {
            var logStringBuilder = new StringBuilder();
            foreach (var kvp in logMessages.OrderBy(kvp => kvp.Value))
            {
                logStringBuilder.AppendLine(string.Format("{0} - {1}", kvp.Value, kvp.Key));
            }
            string str = logStringBuilder.ToString();
            File.AppendAllText(@"c:\temp\logs\_MoneyManagement.txt", str);
            if (sendMail)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(o => Mail(str));
            }

            logMessages.Clear();
        }

        /*
        * This method returns current bar close time or session close, depending on the resolution
        */
        protected TimeSpan GetCloseTime()
        {
            switch (Bars.Request.Resolution.Type)
            {
                case EResolution.Day:
                    foreach (SessionObject s in Bars.Sessions)
                    {
                        if (s.EndDay == DateTime.Now.DayOfWeek)
                            if (s.EndOfSession)
                                return s.EndTime;
                    }
                    break;

                case EResolution.Second:
                case EResolution.Minute:
                case EResolution.Hour:
                    return Bars.TimeValue.TimeOfDay;
            }
            throw new Exception("Can't calculate close time.");  // If the resolution is not time-based, it is not possible to determine bar close time.
        }

        DateTime lastClosedDate;

        protected override void CalcBar()
        {
            if (!Bars.LastBarOnChart) return; // Ignore BackTesting

            //var time = GetCloseTime() - TimeSpan.FromSeconds(30);  // calculate the moment when the calculated coefficient will be requested from each trading strategy.
            // In this case - 30 seconds before the bar close
            var time = triggerTime;

            if (Bars.Status == EBarState.Close)
            {
                LogAndMail("Close event received.");
                foreach (var strategy in PortfolioStrategies)
                {
                    strategy.Status = "Waiting for MMS command...";
                }
                return;
            }

            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            // If it is already necessary to request values from signals, we start the process of receiving values unless the close signal has been received before
            if (currentTime < time || lastClosedDate == Bars.TimeValue.Date || currentTime >= GetCloseTime())
            {
                return;
            }

            // this is needed for the accounts list to be up-to-date and populated with the right values
			TradeManager.ProcessEvents();
			
            var listOfStrategiesWithBuyOrders = new List<IPortfolioStrategy>();

            double sumCRTDR = 0.0;
            var crtdrs = new List<double>();

            double changeInMoney = 0;

            foreach (var strategy in PortfolioStrategies)
            {
                // let's make the most out of our available cash
                // loop through all symbols to calculate sum of all CRTDR values for pro-rata cash distribution
                var crtdr = (double)(strategy.PortfolioData[PortfolioDataKeys.CRTDR] ?? 1.0);
                var wantsToBuy = (bool)(strategy.PortfolioData[PortfolioDataKeys.IWantToBuy] ?? false);
                if (wantsToBuy)
                {
                    listOfStrategiesWithBuyOrders.Add(strategy);
                    crtdrs.Add(crtdr);
                    sumCRTDR += crtdr;
                }
                var wantsToSell = (bool)(strategy.PortfolioData[PortfolioDataKeys.IWantToSell] ?? false);
                if (wantsToSell)
                {
                    strategy.PortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.GenerateOrders_Short;
                    changeInMoney += strategy.Signals[0].CurrentPosition.OpenTrades.Sum(t => t.EntryOrder.Price * t.EntryOrder.Contracts);
                }

                strategy.Status = string.Format("CRTDR: {0:0.00}, {1}", crtdr, wantsToBuy ? "BUY" : (wantsToSell ? "SELL" : "NOP"));
            }

            Log("Assumed cash inflow because of SELLs is: " + changeInMoney);

            // let's only trade the triggering symbols that are "above" average in terms of the CRTDR criteria
            if (listOfStrategiesWithBuyOrders.Count != 0)
            {
                var average = crtdrs.Average();
                var sumOfSquaresOfDifferences = crtdrs.Sum(val => (val - average) * (val - average));
                var stdDev = Math.Sqrt(sumOfSquaresOfDifferences / crtdrs.Count);
                var meanCRTDR = sumCRTDR / listOfStrategiesWithBuyOrders.Count;
                var cutOffCrtdr = meanCRTDR + stdDev * SkalpingFactor;
                Log("Calculated CRTDR std. dev. is {0}, mean is {1} sum of all buying signals is {2} so cut off is {3}", stdDev, meanCRTDR, sumCRTDR, cutOffCrtdr);
                
                foreach (var strategy in listOfStrategiesWithBuyOrders.ToList() /* clone list because we're modifying it below */)
                {
                    bool wantsToBuy = (bool)strategy.PortfolioData[PortfolioDataKeys.IWantToBuy];
                    if (wantsToBuy)
                    {
                        var crtdr = (double)strategy.PortfolioData[PortfolioDataKeys.CRTDR];
                        Log("Symbol {0} has CRTDR {1}", strategy.Signals[0].Bars.Info.Name, crtdr);
                        if (crtdr > cutOffCrtdr)
                        {
                            // adjust previously calculated values
                            listOfStrategiesWithBuyOrders.Remove(strategy);
                            Log("Removing {0} because of CRTDR value {1} which is above the calculated cut-off value of {2}", strategy.Signals[0].Bars.Info.Name, crtdr, cutOffCrtdr);
                            strategy.EntryContracts = 0;
                            sumCRTDR -= crtdr;
                        }
                    }
                }
            }

            double accountEquity = 0;
            if (Environment.IsAutoTradingMode)
            {
                foreach (var account in TradeManager.TradingData.Accounts.Items)
                {
                    accountEquity += account.Equity ?? 0.0;
                    Log(account.ToString());
                }
                
                if(accountEquity == 0) // for some reason this seems to happen sometimes
                {
                    accountEquity = 65000;
                    Log("For some reason the account list appears empty again. We set the assumed equity to " + accountEquity);
                }
            }
            else
            {
                //accountEquity = InitialCapital + Portfolio.NetProfit;
                accountEquity = 70000;
                Log("UTTERLY UNEXPECTED: Environment.IsAutoTradingMode is FALSE?!");
            }

            Log("Assumed account equity is: " + accountEquity);

            double sumInvested = 0.0;
            foreach (var strategy in listOfStrategiesWithBuyOrders)
            {
                var crtdr = (double)strategy.PortfolioData[PortfolioDataKeys.CRTDR];
                double availableCash;
                if (DoReinvestment)
                {
                    availableCash = Math.Min(accountEquity, Math.Max(0, accountEquity + changeInMoney - Portfolio.InvestedCapital));
                }
                else
                {
                    // do not reinvest potential profits, cap @ InitialCapital
                    availableCash = Math.Max(0, Math.Min(InitialCapital, InitialCapital + changeInMoney - Portfolio.InvestedCapital));
                }

                double weightedCRTDR = (sumCRTDR == 0.0 ? (1.0 / (double)listOfStrategiesWithBuyOrders.Count) : (crtdr / sumCRTDR));
                double cashToInvest = availableCash * weightedCRTDR;
                // limit to 33% max. exposure per deal
                cashToInvest = Math.Min(cashToInvest, (accountEquity + changeInMoney) / 3.0);
                sumInvested += cashToInvest;
                var possibleEntryPrice = Math.Abs(strategy.Signals[0].Bars.CloseValue);
                strategy.EntryContracts = (int)(cashToInvest / possibleEntryPrice);
                Log("{0}: (Account balance: {1}) - (Invested capital: {2})) * (Weighted CRTDR: {3}) = {4}$ --> {5}# @ assumed price {6}$ --- available cash: {7}, SUM(CRTDR): {8}", strategy.Signals[0].Bars.Info.Name, accountEquity, Portfolio.InvestedCapital, weightedCRTDR, cashToInvest, strategy.EntryContracts, possibleEntryPrice, availableCash, sumCRTDR);
                if (strategy.EntryContracts > 0)
                {
                    strategy.PortfolioData[PortfolioDataKeys.NumberOfShares] = strategy.EntryContracts;
                    strategy.PortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.GenerateOrders_Long;
                }
                else
                {
                    Log(strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
                }
            }

            Log("Distributed money: {0}$", sumInvested);
            lastClosedDate = Bars.TimeValue.Date;
            //            if(Environment.Optimizing)
            //            {
            //                var result = m_KRatio.Value;
            //                if (Bars.LastBarOnChart)
            //                {
            //                    CustomFitnessValue = result;
            //                }
            //            }
        }

        private static readonly string FromPassword = File.ReadAllText(@"C:\temp\do_not_delete\asdf.txt");
        private static readonly MailAddress FromAddress = new MailAddress("ramanddan.trading@gmail.com", "RamDRom Trading");

        private void Mail(string message)
        {
            if (Environment.IsRealTimeCalc/* && DateTime.Now > DateTime.Now.Date.AddHours(15)*/)
            {
                try
                {
                    using (SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(FromAddress.Address, FromPassword)
                    })
                    using (var mailMessage = new MailMessage()
                    {
                        From = FromAddress,
                        Subject = "Trading day summary: Money Management Signal",
                        Body = message
                    })
                    {
                        mailMessage.To.Add("ramon@winter-berg.com");
                        mailMessage.To.Add("daniel.hegener@gmx.net");
                        mailMessage.To.Add("andermatt.roman@gmail.com");
                        smtp.Send(mailMessage);
                    }
                }
                catch (Exception e)
                {
                    Log("Error sending email: {0}", e.Message);
                }
            }
        }
    }
}