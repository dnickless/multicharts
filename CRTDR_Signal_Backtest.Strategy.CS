using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
    [IOGMode(IOGMode.Disabled)]
    public class CRTDR_Signal_Backtest : PortfolioSignalObject {
        public CRTDR_Signal_Backtest(object _ctx):base(_ctx){}
        private IOrderMarket buyOrder;
        private IOrderMarket sellOrder;
        private IOrderMarket sellShortOrder;
        private IOrderMarket buyToCoverOrder;
        
        [Input]
        public int EmaLong {get; set;}
        
        [Input]
        public int EmaShort  {get; set;}
        
        [Input]
        public int RsiLengthDown {get; set;}
        
        [Input]
        public int RsiLengthFlat {get; set;}
        
        [Input]
        public int RsiLengthUp {get; set;}

        [Input]
        public double LongLimitUp  {get; set;}
        
        [Input]
        public double LongLimitDown  {get; set;}
        
        [Input]
        public double LongLimitFlat {get; set;}
        
        [Input]
        public double ShortLimitDown {get; set;}
        
        [Input]
        public double StopLossLevel {get; set;}
        
        [Input]
        public double RsiSellLevelUp {get; set;}
        
        [Input]
        public double RsiSellLevelDown {get; set;}
        
        [Input]
        public double RsiSellLevelFlat {get; set;}
        
        [Input]
        public double RsiCoverLevelUp {get; set;}
        
        [Input]
        public double RsiCoverLevelDown {get; set;}
        
        [Input]
        public double RsiCoverLevelFlat {get; set;}
        
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathDown;
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathFlat;
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathUp;
        private XAverage xAverageLong;
        private XAverage xAverageShort;
		private bool doReinvestment = true;
        
		//System.IO.StreamWriter file;
		
        protected override void Create() {
            buyOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            sellOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI LE", EOrderAction.Sell, OrderExit.FromAll));
            sellShortOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            buyToCoverOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI SE", EOrderAction.BuyToCover, OrderExit.FromAll));
            cutlersRSIIndicatorMathDown = new CutlersRSIIndicatorMath(this);
            cutlersRSIIndicatorMathFlat = new CutlersRSIIndicatorMath(this);
            cutlersRSIIndicatorMathUp = new CutlersRSIIndicatorMath(this);
            xAverageLong = new XAverage(this);
            xAverageShort = new XAverage(this);
        }
        
        protected override void StartCalc() {
			cutlersRSIIndicatorMathDown.Length = RsiLengthDown;
			cutlersRSIIndicatorMathFlat.Length = RsiLengthFlat;
			cutlersRSIIndicatorMathUp.Length = RsiLengthUp;
            xAverageLong.Price = Bars.Close;
			xAverageLong.Length = EmaLong;
            xAverageShort.Price = Bars.Close;
			xAverageShort.Length = EmaShort;
			//file = System.IO.File.CreateText(@"c:\temp\symbols\" + Bars.Info.Name + ".csv");
        }
        
        protected override void CalcBar(){
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
            var rsiDown = cutlersRSIIndicatorMathDown.CalcNextValue(Bars.CloseValue);
            var rsiFlat = cutlersRSIIndicatorMathFlat.CalcNextValue(Bars.CloseValue);
            var rsiUp = cutlersRSIIndicatorMathUp.CalcNextValue(Bars.CloseValue);
            
            var high = Bars.HighValue;
            var low = Bars.LowValue;
            var close = Bars.CloseValue;
            var crtdr = CRTDRIndicatorMath.CalcNextValue(high, low, close);
	        
			//file.WriteLine(string.Format("{0},{1},{2},{3}", Bars.TimeValue, (int)GetTrend(), Bars.CloseValue, System.Environment.NewLine));
            
			var trend = GetTrend();
			var rsi = trend == Trend.Down ? rsiDown : (trend == Trend.Up ? rsiUp : rsiFlat);
			if(StrategyInfo.MarketPosition == 0)
			{
	            if (GetSignalUp(crtdr, rsi, trend))
	            {
	           		//Output.WriteLine("BUYING on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
	                buyOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
	            }
	            
	            if (GetSignalDown(crtdr, rsi, trend))
	            {
	           		//Output.WriteLine("SELLING short on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
	                //sellShortOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
	            }
			}
			else if(StrategyInfo.MarketPosition > 0)
			{
				// LONG
				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
	            
				if (this.BarsSinceEntry() > 2 && !GetSignalUp(crtdr, rsi, trend))
				{
					sellOrder.Send();
				}
				else if(rsi > (trend == Trend.Down ? RsiSellLevelDown : (trend == Trend.Up ? RsiSellLevelUp : RsiSellLevelFlat)))
				{
					sellOrder.Send();
				}
			}
			else if(StrategyInfo.MarketPosition < 0)
			{
				// SHORT
				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
				
				if(this.BarsSinceEntry() > 2 && !GetSignalDown(crtdr, rsi, trend))
				{
					buyToCoverOrder.Send();
				}
				else if(rsi < (trend == Trend.Down ? RsiCoverLevelDown : (trend == Trend.Up ? RsiCoverLevelUp : RsiCoverLevelFlat)))
				{
					buyToCoverOrder.Send();
				}
			}
        }
        
		private enum Trend
		{
			Up = 1,
			Down = -1,
			Flat = 0
		}
		
		private Trend GetTrend()
		{
            if (xAverageLong.Value > xAverageShort.Value && xAverageShort.Value > Bars.CloseValue)
            {
				return Trend.Down;
            }
            else if(Bars.CloseValue > xAverageLong.Value && xAverageLong.Value < xAverageShort.Value)
            {
				return Trend.Up;
            }
			else return Trend.Flat;
		}
		
        private bool GetSignalUp(double crtdr, double rsi, Trend trend)
        {
			if(rsi < 40)
			{
				if(trend == Trend.Up)
				{
					if(crtdr * 100 + rsi <= LongLimitUp) return true;
				}
				else if(trend == Trend.Down)
				{
					if(crtdr * 100 + rsi <= LongLimitDown) return true;
				}
				else
				{
					if(crtdr * 100 + rsi <= LongLimitFlat) return true;
				}
			}
			
			return false;
		}
		
        private bool GetSignalDown(double crtdr, double rsi, Trend trend)
        {
			if(rsi > 60 && trend == Trend.Down)
			{
				if(crtdr * 100 + rsi >= 200 - ShortLimitDown) return true;
			}
            
			return false;
		}
	}
}