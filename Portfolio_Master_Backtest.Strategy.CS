using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public class Portfolio_Master_Backtest : PortfolioSignalObject
    {
        [Input]
        public double SkalpingFactor { get; set; }

        private K_RatioMath m_KRatio;

        private DateTime m_LastTraceTime = DateTime.MinValue;

        public Portfolio_Master_Backtest(object _ctx)
            : base(_ctx)
        {
            SkalpingFactor = 1.1;
        }

        protected override void Create()
        {
            m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
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
                DumpLog();
            }
        }

        [Input]
        public bool DoReinvestment { get; set; }

        private readonly Dictionary<string, DateTime> logMessages = new Dictionary<string, DateTime>();

        private void Log(string message, params object[] parameters)
        {
            if (!Environment.Optimizing)
            {
                try
                {
                    var str = string.Format("{0}: {1}", Bars.TimeValue, string.Format(message, parameters));

                    //Output.WriteLine(string.Format("{0} - {1}", DateTime.Now, str));

                    logMessages[str] = DateTime.Now;

                    if(logMessages.Count >= 1000)
                    {
                        DumpLog();
                    }
                }
                catch (Exception e)
                {
                    Output.WriteLine(e.ToString());
                }
            }
        }

        private void DumpLog()
        {
            var logStringBuilder = new StringBuilder();
            foreach (var kvp in logMessages.OrderBy(kvp => kvp.Value))
            {
                logStringBuilder.AppendLine(string.Format("{0} - {1}", kvp.Value, kvp.Key));
            }
            string str = logStringBuilder.ToString();
            File.AppendAllText(@"c:\temp\logs_backtest\_MoneyManagement.txt", str);

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

            double accountEquity = InitialCapital + Portfolio.NetProfit;

            double availableCash;
            if (DoReinvestment)
            {
                availableCash = Math.Min(accountEquity, Math.Max(0, accountEquity + changeInMoney - Portfolio.InvestedCapital /* always zero in backtesting */));
            }
            else
            {
                // do not reinvest potential profits, cap @ InitialCapital
				
				
				// STATUS: Money Mgmt. not good, reinvesting partially.
                
				
				availableCash = Math.Max(0, Math.Min(InitialCapital, InitialCapital + changeInMoney - Portfolio.InvestedCapital /* always zero in backtesting */));
            }

            double sumInvested = 0.0;
			
			double avgCRTDR = 0;
			if(listOfStrategiesWithBuyOrders.Count != 0)
			{
				avgCRTDR = sumCRTDR / (listOfStrategiesWithBuyOrders.Count);
			}
			
			foreach (var strategy in listOfStrategiesWithBuyOrders)
            {
                var crtdr = (double)strategy.PortfolioData[PortfolioDataKeys.CRTDR];
				double weightedCRTDR = sumCRTDR == 0.0 ? 1.0 / listOfStrategiesWithBuyOrders.Count : (avgCRTDR + crtdr) / sumCRTDR / 2.0;
                double cashToInvest = listOfStrategiesWithBuyOrders.Count == 1 ? availableCash : (availableCash * weightedCRTDR);
                // limit to 33% max. exposure per deal
                cashToInvest = Math.Min(cashToInvest, (accountEquity + changeInMoney) / 3.0);
                sumInvested += cashToInvest;
                var possibleEntryPrice = Math.Abs(strategy.Signals[0].Bars.CloseValue);
                strategy.EntryContracts = (int)(cashToInvest / possibleEntryPrice);
                Log("{0}: (Account equity: {1}) - (Invested capital: {2})) * (Weighted CRTDR: {3}) = {4}$ --> {5}# @ assumed price {6}$ --- available cash: {7}, SUM(CRTDR): {8}", strategy.Signals[0].Bars.Info.Name, accountEquity, Portfolio.InvestedCapital, weightedCRTDR, cashToInvest, strategy.EntryContracts, possibleEntryPrice, availableCash, sumCRTDR);
                if (strategy.EntryContracts <= 0)
                {
                    Log(strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
                }
            }

            Log("Distributed money: {0}$", sumInvested);
            if(Environment.Optimizing)
            {
                var result = m_KRatio.Value;
                if (Bars.LastBarOnChart)
                {
                    CustomFitnessValue = result;
                }
            }
        }
    }
}