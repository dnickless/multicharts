using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
    [IOGMode(IOGMode.Enabled)]
	//[AllowSendOrdersAlways(true)]
    public class CRTDR_Signal : PortfolioSignalObject {
        public CRTDR_Signal(object _ctx):base(_ctx){}
        private IOrderMarket buyOrder;
        private IOrderMarket sellOrder;
        private IOrderMarket sellShortOrder;
        private IOrderMarket buyToCoverOrder;
        private TimeSpan closeTime = new TimeSpan(15, 59, 30);
        private bool executedInThisBar;
        
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
		private bool doReinvestment = false;
        
        protected override void Create() {
            buyOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            sellOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI LE", EOrderAction.Sell, OrderExit.FromAll));
            sellShortOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            buyToCoverOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI SE", EOrderAction.BuyToCover, OrderExit.FromAll));
            cutlersRSIIndicatorMathDown = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathFlat = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathUp = new CutlersRSIIndicatorMath(this, 1);
            xAverageLong = new XAverage(this, 1);
            xAverageShort = new XAverage(this, 1);
        }
        
        protected override void StartCalc() {
			cutlersRSIIndicatorMathDown.Length = RsiLengthDown;
			cutlersRSIIndicatorMathFlat.Length = RsiLengthFlat;
			cutlersRSIIndicatorMathUp.Length = RsiLengthUp;
            xAverageLong.Price = BarsOfData(1).Close;
			xAverageLong.Length = EmaLong;
            xAverageShort.Price = BarsOfData(1).Close;
			xAverageShort.Length = EmaShort;
			executedInThisBar = false;
			
			//ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(15));
        }
        
        // http://www.tradingcode.net/multicharts-net/trading-strategies/exit-strategy-position-prior-close/
        private bool TimeToEnter()
        {
            // On real-time data, we use DateTime.Now
            // Else, when backtesting, use the bar time
            return (Environment.IsRealTimeCalc ? DateTime.Now : Bars.TimeValue).TimeOfDay >= closeTime;
        }
        
        protected override void CalcBar(){
//            if(Environment.IsRealTimeCalc)
//			{
//				Output.WriteLine("{0} - TimeToEnter() {1} executedInThisBar {2} Environment.IsRealTimeCalc {3} Bars.TimeValue {4}", DateTime.Now, TimeToEnter(), executedInThisBar, Environment.IsRealTimeCalc, Bars.TimeValue);
//			}
			
            var timeToEnter = TimeToEnter();
  			
			ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(15));
			
  			if(!timeToEnter)
 			{
				//Output.WriteLine("{0} - Not yet TimeToEnter() {1} executedInThisBar {2} Environment.IsRealTimeCalc {3} Bars.TimeValue {4}", DateTime.Now, TimeToEnter(), executedInThisBar, Environment.IsRealTimeCalc, Bars.TimeValue);
 				executedInThisBar = false;
 				return;
 			}
  			
  			if(Environment.IsRealTimeCalc && executedInThisBar)
 			{
				return;
			}
			
            if(!Environment.IsRealTimeCalc && Bars.TimeValue.Date.Equals(DateTime.Now.Date))
			{
                return;
			}
            
			executedInThisBar = true;
			
            // strategy logic 
            var rsiDown = cutlersRSIIndicatorMathDown.CalcNextValue(BarsOfData(1).CloseValue);
            var rsiFlat = cutlersRSIIndicatorMathFlat.CalcNextValue(BarsOfData(1).CloseValue);
            var rsiUp = cutlersRSIIndicatorMathUp.CalcNextValue(BarsOfData(1).CloseValue);
            
            var high = BarsOfData(1).HighValue;
            var low = BarsOfData(1).LowValue;
            var close = BarsOfData(1).CloseValue;
            
            //Output.WriteLine("Bars.TimeValue {0}, high {1} low {2} close", Bars.TimeValue, high, low, close);
            var crtdr = CRTDRIndicatorMath.CalcNextValue(high, low, close);
			
			var trend = GetTrend();
			var rsi = trend == Trend.Down ? rsiDown : (trend == Trend.Up ? rsiUp : rsiFlat);
			
       		Output.WriteLine("{8}: Executing strategy {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
			if(StrategyInfo.MarketPosition == 0)
			{
	            if (GetSignalUp(crtdr, rsi, trend))
	            {
	           		Output.WriteLine("{8}: LONG on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
	                buyOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 1.0 / Bars.CloseValue));
	            }
//				else if (GetSignalDown(crtdr, rsi, trend))
//	            {
//	           		Output.WriteLine("SHORT on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
//					sellShortOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
//	            }
				else
				{
	           		Output.WriteLine("NOTHING(B) on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
				}
			}
			else if(StrategyInfo.MarketPosition > 0)
			{
				// LONG
				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
	            
				if (this.BarsSinceEntry() > 2 && !GetSignalUp(crtdr, rsi, trend))
				{
	           		Output.WriteLine("{8}: SELL1 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
					sellOrder.Send();
				}
				else if(rsi > (trend == Trend.Down ? RsiSellLevelDown : (trend == Trend.Up ? RsiSellLevelUp : RsiSellLevelFlat)))
				{
	           		Output.WriteLine("{8}: SELL2 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
					sellOrder.Send();
				}
			}
//			else if(StrategyInfo.MarketPosition < 0)
//			{
//				// SHORT
//				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
//				
//				if(this.BarsSinceEntry() > 2 && !GetSignalDown(crtdr, rsi, trend))
//				{
//	           		//Output.WriteLine("{8}: COVER1 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
//					buyToCoverOrder.Send();
//				}
//				else if(rsi < (trend == Trend.Down ? RsiCoverLevelDown : (trend == Trend.Up ? RsiCoverLevelUp : RsiCoverLevelFlat)))
//				{
//	           		//Output.WriteLine("{8}: COVER2 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value, Bars.Info.Name);
//					buyToCoverOrder.Send();
//				}
//			}
        }
        
		private enum Trend
		{
			Up = 1,
			Down = -1,
			Flat = 0
		}
		
		private Trend GetTrend()
		{
            if (xAverageLong.Value > xAverageShort.Value && xAverageShort.Value > BarsOfData(1).CloseValue)
            {
				return Trend.Down;
            }
            else if(BarsOfData(1).CloseValue > xAverageLong.Value && xAverageLong.Value < xAverageShort.Value)
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
		
        // Recalculate the last bar when performing the recalculation
        protected override void OnRecalcLastBarAfterEvent()
        {
            CalcBar();
        }
		
		protected override void OnBrokerPositionChange()
		{
			Output.WriteLine("{2}: MarketPositionAtBroker={0}, AvgEntryPriceAtBroker={1}", Bars.Info.Name, StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker);
		}
	}
}