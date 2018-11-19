using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public class CRTDR_Signal_Backtest : PortfolioSignalObject
    {
        private const int recalcFrequency = 10;
        public CRTDR_Signal_Backtest(object _ctx) : base(_ctx) { }
        private IOrderMarket buyOrder;
        private IOrderMarket sellOrder;

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
        public double TakeProfitLevel { get; set; }

        [Input]
        public double LongLimitUp { get; set; }

        [Input]
        public double LongLimitDown { get; set; }

        [Input]
        public double LongLimitFlat { get; set; }

        [Input]
        public double StopLossLevel { get; set; }

        [Input]
        public double RsiSellLevelUp { get; set; }

        [Input]
        public double RsiSellLevelDown { get; set; }

        [Input]
        public double RsiSellLevelFlat { get; set; }

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
                DumpLog();
            }
        }

        private readonly Dictionary<string, DateTime> logMessages = new Dictionary<string, DateTime>();

        private void Log(string message, params object[] parameters)
        {
            if (!Environment.Optimizing)
            {
                try
                {
                    var str = string.Format("{0} - {1}: {2}", Bars.TimeValue, Bars.Info.Name, string.Format(message, parameters));

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
            File.AppendAllText(@"c:\temp\logs_backtest\" + Bars.Info.Name + ".txt", str);

            logMessages.Clear();
        }

        protected override void CalcBar()
        {
			CurSpecOrdersMode = ESpecOrdersMode.PerPosition;
			
            MyPortfolioData[PortfolioDataKeys.CRTDR] = 1.0;
            MyPortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.None;
            MyPortfolioData[PortfolioDataKeys.IWantToBuy] = false;
            MyPortfolioData[PortfolioDataKeys.IWantToSell] = false;
            
            Log("Calc Reason: {0}, Bar Status: {1}, Open: {2}, High: {3}, Low: {4}, Close: {5}", Environment.CalcReason, Bars.Status, Bars.OpenValue, Bars.HighValue, Bars.LowValue, Bars.CloseValue);

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

            Log("Calculating strategy, high {0} low {1} close {2} rsi {3} crtdr {4} xAverageLong {5} xAverageShort {6}", high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue);

            if (StrategyInfo.MarketPosition == 0)
            {
                if (GetSignalUp(crtdr, rsi, trend))
                {
                    var numberOfShares = Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / Bars.CloseValue);
                    MyPortfolioData[PortfolioDataKeys.IWantToBuy] = true;
                    buyOrder.Send(numberOfShares);
					GenerateOrAdjustStopLosses();
                    Log("Generated Buy: {0}# - may be overwritten by MMS", numberOfShares);
                }
            }
            else if (StrategyInfo.MarketPosition > 0)
            {
                // WE ARE LONG
                
                var barsSinceEntry = Math.Min(1, this.BarsSinceEntry());
                var openProfit = CurrentPosition.OpenProfit;
                var signalUp = GetSignalUp(crtdr, rsi, trend);
                var rsiSellLevel = GetRsiSellLevel(trend);
                Log(CurrentPosition.Value.ToString() + " - " + openProfit.ToString());
                Log("Should we sell? Bars since entry: {0}, open profit: {1}, signal up: {2}, rsi: {3}, rsiSellLevel: {4}", barsSinceEntry, openProfit, signalUp, rsi, rsiSellLevel);

                // close non-profitable positions straight away because we have picked a loser and need to free up money for new deals
                if (openProfit < 0)
                {
                    Log("Loser cut! Assumed loss: {0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send("Loser cut");
                    Log("Generated Sell.");
                }
                else if ((CurrentPosition.Value * close) / (CurrentPosition.Value * close - openProfit) > TakeProfitLevel)
                {
                    Log("Take profit! Assuming to cash in ~{0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send("Take profit");
                    Log("Generated Sell.");
                }
                else if (barsSinceEntry > 2 && !signalUp)
                {
                    Log("SELL! Bars since entry > 2 and no up signal. Assuming to cash in ~{0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send("barsSinceEntry > 2 && !signalUp");
                    Log("Generated Sell.");
                }
                else if (rsi > rsiSellLevel)
                {
                    Log("SELL! RSI condition satisfied, we take the profit (~{0}$) and run!", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send("rsi > rsiSellLevel");
                    Log("Generated Sell.");
                }
            }
        }

		private void GenerateOrAdjustStopLosses()
		{
			if (CurSpecOrdersMode != ESpecOrdersMode.PerPosition) {
				throw new Exception("Incorrect CurSpecOrdersMode. ESpecOrdersMode.PerPosition required.");
			}
			
            double amount = StopLossLevel / (Bars.Point * Bars.Info.MinMove);
            amount *= (StrategyInfo.MarketPosition > 0) ? StrategyInfo.AvgEntryPrice : Bars.Close[0];
			
			// Inserted as per Svetlana's request: https://www.multicharts.com/discussion/viewtopic.php?p=130893&sid=428630f2f75e47b752f6eb1a6577e158#p130893
			if (Environment.IsAutoTradingMode && Bars.LastBarOnChart)
                Log("sysDT= {0}, Name= {1}, barDT= {2}, BStatus= {3}, MP= {4}, MPBS= {5}, EP= {6}, EPBS= {7}, SL @ {8}",
                            DateTime.Now.ToString("MM/dd/yy hh:mm:ss.fff"),
                            Bars.Info.Name,
                            Bars.BarUpdateTime.ToString("MM/dd/yy hh:mm:ss.fff"),
                            Bars.Status,
                            StrategyInfo.MarketPosition,
                            StrategyInfo.MarketPositionAtBrokerForTheStrategy,
                            StrategyInfo.AvgEntryPrice,
                            StrategyInfo.AvgEntryPriceAtBrokerForTheStrategy,
                            amount);
					
            Log("Generating stop loss @ {0}", amount);
            GenerateStopLossPt(amount);
			
            Log("Generating stop loss starting at {0}", amount);
            GeneratePercentTrailingPt(amount, 50);
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
    }
}