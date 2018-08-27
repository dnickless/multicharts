using System;
using System.Collections;
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
    public class CRTDR_Signal : PortfolioSignalObject
    {
        private const int recalcFrequency = 10;
        public CRTDR_Signal(object _ctx) : base(_ctx) { }
        private IOrderMarket buyOrder;
        private IOrderMarket sellOrder;
        //        private IOrderMarket sellShortOrder;
        //        private IOrderMarket buyToCoverOrder;

        //        [Input]
        //        public int RsiUpMinimumForShort {get; set;}

        [Input]
        public int EmaLong { get; set; }

        [Input]
        public int EmaShort { get; set; }

        [Input]
        public int RsiLengthDown { get; set; }

        [Input]
        public int RsiLengthFlat { get; set; }

        [Input]
        public int RsiLengthUp { get; set; }

        [Input]
        public double LongLimitUp { get; set; }

        [Input]
        public double LongLimitDown { get; set; }

        [Input]
        public double LongLimitFlat { get; set; }

        //        [Input]
        //        public double ShortLimitDown {get; set;}

        [Input]
        public double StopLossLevel { get; set; }

        [Input]
        public double RsiSellLevelUp { get; set; }

        [Input]
        public double RsiSellLevelDown { get; set; }

        [Input]
        public double RsiSellLevelFlat { get; set; }

        //        [Input]
        //        public double RsiCoverLevelUp {get; set;}

        //        [Input]
        //        public double RsiCoverLevelDown {get; set;}

        //        [Input]
        //       public double RsiCoverLevelFlat {get; set;}

        private CutlersRSIIndicatorMath cutlersRSIIndicatorMathDown;
        private CutlersRSIIndicatorMath cutlersRSIIndicatorMathFlat;
        private CutlersRSIIndicatorMath cutlersRSIIndicatorMathUp;
        private XAverageThatWorks xAverageLong;
        private XAverageThatWorks xAverageShort;
        private bool doReinvestment = false;

        private DateTime lastOrderGenerated;

        string symbolName;

        protected override void Create()
        {
            buyOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            sellOrder = OrderCreator.MarketThisBar(new SOrderParameters(EOrderAction.Sell));
            //sellShortOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            //buyToCoverOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI SE", EOrderAction.BuyToCover, OrderExit.FromAll));
            cutlersRSIIndicatorMathDown = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathFlat = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathUp = new CutlersRSIIndicatorMath(this, 1);
            xAverageLong = new XAverageThatWorks(this, 1);
            xAverageShort = new XAverageThatWorks(this, 1);
        }

        protected override void StartCalc()
        {
            cutlersRSIIndicatorMathDown.Length = RsiLengthDown;
            cutlersRSIIndicatorMathFlat.Length = RsiLengthFlat;
            cutlersRSIIndicatorMathUp.Length = RsiLengthUp;
            xAverageLong.Price = Bars.Close;
            xAverageLong.Length = EmaLong;
            xAverageShort.Price = Bars.Close;
            xAverageShort.Length = EmaShort;
            symbolName = Bars.Info.Name;
        }

        protected override void StopCalc()
        {
            if (!Environment.Optimizing)
            {
                Output.WriteLine("Dumping log for {0}", Bars.Info.Name);
                DumpFileLog();
                DumpMailLog();
            }
        }

        private readonly StringBuilder _logStringBuilder = new StringBuilder();
        private readonly Dictionary<string, DateTime> emailLogMessages = new Dictionary<string, DateTime>();
        private DateTime _lastLogEntry = DateTime.MinValue;
        private bool _throttledLoggingActive = false;
        private string _lastLogMessage;
        private bool _forceMailLog;
        
        private void UpdateLogThrottling()
        {
            var now = DateTime.Now;
            _forceMailLog = Bars.Status == EBarState.Close;
            if(_lastLogEntry <= now || _forceMailLog)
            {
                _throttledLoggingActive = true;
                double frequency = now.TimeOfDay > new TimeSpan(15, 57, 00) && now.TimeOfDay < new TimeSpan(16, 00, 00) ? 0.25 : 1;
                _lastLogEntry = now.AddMinutes(frequency);
            }
            else
            {
                _throttledLoggingActive = false;
            }
        }
        
        private void LogThrottled(string message, params object[] parameters)
        {
            if(_throttledLoggingActive)
            {
                Log(message, parameters);
            }
        }
        
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
                        var logMessage = string.Format("{0} - {1}: {2}", Bars.TimeValue, Bars.Info.Name, string.Format(message, parameters));
                        if(string.Equals(message, _lastLogMessage))
                        {
                            return;
                        }
                        
                        _lastLogMessage = logMessage;
                        
                        var fullLogString = string.Format("{0} - {1}", DateTime.Now, logMessage);
                        
                        //Output.WriteLine(fullLogString);
                        _logStringBuilder.AppendLine(fullLogString);
                        
                        if(_logStringBuilder.Length >= 1000)
                        {
                            DumpFileLog();
                        }
                        
                        if (sendMail)
                        {
                            emailLogMessages[logMessage] = DateTime.Now;
                        }
                    }
                    catch (Exception e)
                    {
                        Output.WriteLine(e.ToString());
                    }
                }
            }
        }

        private void DumpFileLog()
        {
            File.AppendAllText(@"c:\temp\logs\" + Bars.Info.Name + ".txt", _logStringBuilder.ToString());
            _logStringBuilder.Clear();
        }
        
        private void DumpMailLog()
        {
            if (emailLogMessages.Count != 0)
            {
                var logStringBuilder = new StringBuilder();
                foreach (var kvp in emailLogMessages.OrderBy(kvp => kvp.Value))
                {
                    logStringBuilder.AppendLine(string.Format("{0} - {1}", kvp.Value, kvp.Key));
                }
                string str = logStringBuilder.ToString();
                System.Threading.ThreadPool.QueueUserWorkItem(o => Mail(str));

                emailLogMessages.Clear();
            }
        }

        protected override void CalcBar()
        {
            if (!Environment.IsRealTimeCalc) // There is no need to calculate on historical data, that is why backtesting is ignored
            {
                // As soon as backtesting is finished, the signal gets immediately calculated by the timer.
                // It is necessary to calculate the CRTDR and other values which can be requested by MM-signal at any moment.
                ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(recalcFrequency));
                return;
            }
            else
            {
                System.Diagnostics.Debugger.Launch();
            }

            UpdateLogThrottling();
            
            var high = Bars.HighValue;
            var low = Bars.LowValue;
            var close = Bars.CloseValue;

            var crtdr = CRTDRIndicatorMath.CalcNextValue(high, low, close);

            var xAverageLongValue = xAverageLong.Value;
            var xAverageShortValue = xAverageShort.Value;

            var trend = GetTrend(close, xAverageLongValue, xAverageShortValue);

            var rsiDown = cutlersRSIIndicatorMathDown.Value;
            var rsiFlat = cutlersRSIIndicatorMathFlat.Value;
            var rsiUp = cutlersRSIIndicatorMathUp.Value;

            var rsi = trend == Trend.Down ? rsiDown : (trend == Trend.Up ? rsiUp : rsiFlat);

            MyPortfolioData[PortfolioDataKeys.IWantToBuy] = false;
            MyPortfolioData[PortfolioDataKeys.IWantToSell] = false;
            MyPortfolioData[PortfolioDataKeys.CRTDR] = crtdr;
            
            LogThrottled("Reason: {0}, Status: {1}, Open: {2}, High: {3}, Low: {4}, Close: {5}, RSI: {6}, CRTDR: {7}, xAvgLong: {8}, xAvgShort: {9}", Environment.CalcReason, Bars.Status, Bars.OpenValue, Bars.HighValue, Bars.LowValue, Bars.CloseValue, rsi, crtdr, xAverageLongValue, xAverageShortValue);

            if (StrategyInfo.MarketPosition == 0)
            {
                if (GetSignalUp(crtdr, rsi, trend))
                {
                    //var numLots = Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / Bars.CloseValue);
                    MyPortfolioData[PortfolioDataKeys.IWantToBuy] = true;
                    LogThrottled("Signal says: We would like to buy @ {0}$", Bars.CloseValue);
                }
                //                else if (GetSignalDown(crtdr, rsi, trend))
                //                {
                //                       Output.WriteLine("SHORT on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue);
                //                    MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = -Bars.CloseValue;
                //                    sellShortOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
                //                }
                //                else
                //                {
                //                       Log("---  FLAT - NOP ---");
                //                }
            }
            else if (StrategyInfo.MarketPosition > 0)
            {
                // WE ARE LONG
                GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
                GeneratePercentTrailing(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * 0.01, 50);

                //var barsSinceEntry = Bars.CurrentBar - CurrentPosition.OpenTrades[0].EntryOrder.BarNumber;
                var barsSinceEntry = Math.Min(1, this.BarsSinceEntry());
                var openProfit = CurrentPosition.OpenProfit;
                var signalUp = GetSignalUp(crtdr, rsi, trend);
                var rsiSellLevel = GetRsiSellLevel(trend);
                LogThrottled("Should we sell? Bars since entry: {0}, open profit: {1}, signal up: {2}, rsi: {3}, rsiSellLevel: {4}", barsSinceEntry, openProfit, signalUp, rsi, rsiSellLevel);

                // close non-profitable positions straight away because we have picked a loser and need to free up money for new deals
                if (openProfit < 0)
                {
                    LogThrottled("Loser cut! Assumed loss: {0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                }
                else if (barsSinceEntry > 2 && !signalUp)
                {
                    LogThrottled("SELL! Bars since entry > 2 and no up signal. Assuming to cash in ~{0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                }
                else if (rsi > rsiSellLevel)
                {
                    LogThrottled("SELL! RSI condition satisfied, we take the profit (~{0}$) and run!", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                }
                else
                {
                    //Log("---  LONG - NOP ---");
                }
            }
            //            else if(marketPosition < 0)
            //            {
            //                // WE ARE SHORT
            //                GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(marketPosition) * StopLossLevel);
            //                
            //                if(this.BarsSinceEntry() > 2 && !GetSignalDown(crtdr, rsi, trend))
            //                {
            //                       //Output.WriteLine("{8}: COVER1 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue, Bars.Info.Name);
            //                    buyToCoverOrder.Send();
            //                }
            //                else if(rsi < (trend == Trend.Down ? RsiCoverLevelDown : (trend == Trend.Up ? RsiCoverLevelUp : RsiCoverLevelFlat)))
            //                {
            //                       //Output.WriteLine("{8}: COVER2 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue, Bars.Info.Name);
            //                    buyToCoverOrder.Send();
            //                }
            //                else
            //                {
            //                       Log("--- SHORT - NOP ---");
            //                }
            //            }

            StrategyEvents cmd = StrategyEvents.None;
            object obj = MyPortfolioData[PortfolioDataKeys.MoneyManagementCommand];   // Check, if there are any MM-signal events available

            if (obj != null)
            {
                cmd = (StrategyEvents)obj;
            }

            //Log("Got command {0}", cmd);

            switch (cmd) // Execute specific commands, depending on the event
            {
                case StrategyEvents.GenerateOrders_Long:
                    int numberOfShares = Convert.ToInt32(MyPortfolioData[PortfolioDataKeys.NumberOfShares]);
                    buyOrder.Send(numberOfShares);
                    lastOrderGenerated = DateTime.Now.Date;
                    LogThrottled("Generated Buy: {0}#", numberOfShares);
                    break;

                case StrategyEvents.GenerateOrders_Short:
                    sellOrder.Send();
                    lastOrderGenerated = DateTime.Now.Date;
                    LogThrottled("Generated Sell.");
                    break;

                case StrategyEvents.None:
                    ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(recalcFrequency));
                    break;
            }

            // During calculation at the bar close, global variables, used to exchange messages between the current signal and MM-signal should be reset
            if (Bars.Status == EBarState.Close)
            {
                Log(lastOrderGenerated == DateTime.Now.Date, "Close event received."); // only send email output if trade has been sent on this day
                DumpFileLog();
                DumpMailLog();

                MyPortfolioData[PortfolioDataKeys.CRTDR] = 1.0;
                MyPortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.None;
                MyPortfolioData[PortfolioDataKeys.IWantToBuy] = false;
                MyPortfolioData[PortfolioDataKeys.IWantToSell] = false;
                MyPortfolioData[PortfolioDataKeys.NumberOfShares] = 0;
                ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(recalcFrequency)); // And start the timer to wait for the new commands from MM-signal
            }
        }

        private double GetRsiSellLevel(Trend trend)
        {
            return trend == Trend.Down ? RsiSellLevelDown : (trend == Trend.Up ? RsiSellLevelUp : RsiSellLevelFlat);
        }

        private enum Trend
        {
            Up = 1,
            Down = -1,
            Flat = 0
        }

        private static Trend GetTrend(double close, double xAverageLongValue, double xAverageShortValue)
        {
            if (close > xAverageLongValue && xAverageLongValue < xAverageShortValue)
            {
                return Trend.Up;
            }
            if (xAverageLongValue > xAverageShortValue && xAverageShortValue > close)
            {
                return Trend.Down;
            }
            return Trend.Flat;
        }

        private bool GetSignalUp(double crtdr, double rsi, Trend trend)
        {
            if (trend == Trend.Up)
            {
                if (crtdr * 100 + rsi <= LongLimitUp) return true;
            }
            else if (trend == Trend.Down)
            {
                if (crtdr * 100 + rsi <= LongLimitDown) return true;
            }
            else
            {
                if (crtdr * 100 + rsi <= LongLimitFlat) return true;
            }

            return false;
        }

        //        private double GetMarketPosition()
        //        {
        //            return this.Environment.IsAutoTradingMode ? StrategyInfo.MarketPositionAtBrokerForTheStrategy : StrategyInfo.MarketPosition;
        //        }

        //        private bool GetSignalDown(double crtdr, double rsi, Trend trend)
        //        {
        //            if(rsi >= RsiUpMinimumForShort && trend == Trend.Down)
        //            {
        //                if(crtdr * 100 + rsi >= 200 - ShortLimitDown) return true;
        //            }
        //            
        //            return false;
        //        }

        protected override void OnOrderRejected(EOrderAction action, OrderCategory category, int quantity, double stopPrice, double limitPrice)
        {
            LogAndMail("Order rejected: {0} {1} ({2} @ {3}/{4}).", action, quantity, category, stopPrice, limitPrice);
			DumpFileLog();
			DumpMailLog();
            ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(recalcFrequency));                                            // If there were no instructions from MM-signal, we will keep waiting for them.
        }

        protected override void OnBrokerStategyOrderFilled(bool isBuy, int quantity, double avgFillPrice)
        {
            double pAndL = quantity * (avgFillPrice - StrategyInfo.AvgEntryPriceAtBroker);
            LogAndMail("Order filled: {0} {1} @ avg. {2} (==> {3}). MarketPositionAtBroker={4}, AvgEntryPriceAtBroker={5}", isBuy ? "BUY" : "SELL", quantity, avgFillPrice, isBuy ? "Investing " + quantity * avgFillPrice : ("P/L " + pAndL + (pAndL > 0 ? " - yeah, baby!" : " - ouch!")), StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker);
			DumpFileLog();
			DumpMailLog();
            ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(recalcFrequency));                                            // If there were no instructions from MM-signal, we will keep waiting for them.
        }

        protected override void OnBrokerPositionChange()
        {
            Log("MarketPositionAtBroker={0}, AvgEntryPriceAtBroker={1}, MarketPositionAtBrokerForTheStrategy={2}, AvgEntryPriceAtBrokerForTheStrategy={3}, MarketPosition={4}, AvgEntryPrice={5}", StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker, StrategyInfo.MarketPositionAtBrokerForTheStrategy, StrategyInfo.AvgEntryPriceAtBrokerForTheStrategy, StrategyInfo.MarketPosition, StrategyInfo.AvgEntryPrice);
        }

        protected override void OnRecalcLastBarAfterEvent()
        {
            CalcBar();
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
                        Subject = string.Format("Trading day summary: {0}", symbolName),
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