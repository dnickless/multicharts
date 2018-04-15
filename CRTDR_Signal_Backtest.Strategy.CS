﻿using System;
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
                Output.WriteLine("Dumping log for {0}", Bars.Info.Name);
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

					Output.WriteLine(string.Format("{0} - {1}", DateTime.Now, str));

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
            File.AppendAllText(@"c:\temp\logs\" + Bars.Info.Name + ".txt", str);

            logMessages.Clear();
        }

        protected override void CalcBar()
        {
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
                    Log("Generated Buy: {0}# - may be overwritten by MMS", numberOfShares);
                }
            }
            else if (StrategyInfo.MarketPosition > 0)
            {
                // WE ARE LONG
                GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);

                var barsSinceEntry = Math.Min(1, this.BarsSinceEntry());
                var openProfit = CurrentPosition.OpenProfit;
                var signalUp = GetSignalUp(crtdr, rsi, trend);
                var rsiSellLevel = GetRsiSellLevel(trend);
                Log("Should we sell? Bars since entry: {0}, open profit: {1}, signal up: {2}, rsi: {3}, rsiSellLevel: {4}", barsSinceEntry, openProfit, signalUp, rsi, rsiSellLevel);

                // close non-profitable positions straight away because we have picked a loser and need to free up money for new deals
                if (openProfit < 0)
                {
                    Log("Loser cut! Assumed loss: {0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send();
                    Log("Generated Sell.");
                }
                else if (barsSinceEntry > 2 && !signalUp)
                {
                    Log("SELL! Bars since entry > 2 and no up signal. Assuming to cash in ~{0}$", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send();
                    Log("Generated Sell.");
                }
                else if (rsi > rsiSellLevel)
                {
                    Log("SELL! RSI condition satisfied, we take the profit (~{0}$) and run!", CurrentPosition.OpenProfit);
                    MyPortfolioData[PortfolioDataKeys.IWantToSell] = true;
                    sellOrder.Send();
                    Log("Generated Sell.");
                }
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
    }
}