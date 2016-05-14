using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
    public class CRTDR_Signal : PortfolioSignalObject {
        public CRTDR_Signal(object _ctx):base(_ctx){}
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
            xAverageLong.Price = Bars.Close;
			xAverageLong.Length = EmaLong;
            xAverageShort.Price = Bars.Close;
			xAverageShort.Length = EmaShort;
        }
        
		private void Log(string message, params object[] parameters)
		{
			//if(!Environment.Optimizing) Output.WriteLine("{0} - {1} - ({2}) - {3}: {4}", DateTime.Now, Bars.TimeValue, Environment.IsRealTimeCalc ? "REALTIME" : "HISTORIC", Bars.Info.Name, string.Format(message, parameters));
		}
		
        protected override void CalcBar(){
            var rsiDown = cutlersRSIIndicatorMathDown.CalcNextValue(Bars.CloseValue);
            var rsiFlat = cutlersRSIIndicatorMathFlat.CalcNextValue(Bars.CloseValue);
            var rsiUp = cutlersRSIIndicatorMathUp.CalcNextValue(Bars.CloseValue);
            
            var high = Bars.HighValue;
            var low = Bars.LowValue;
            var close = Bars.CloseValue;
            
            var crtdr = CRTDRIndicatorMath.CalcNextValue(high, low, close);
			
			var trend = GetTrend();
			var rsi = trend == Trend.Down ? rsiDown : (trend == Trend.Up ? rsiUp : rsiFlat);
			
       		Log("Calculating strategy, high {0} low {1} close {2} rsi {3} crtdr {4} xAverageLong {5} xAverageShort {6}", high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
			if(StrategyInfo.MarketPosition == 0)
			{
	            if (GetSignalUp(crtdr, rsi, trend))
	            {
					var numLots = Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / Bars.CloseValue);
	           		Log("!!!LONG !!! Attempting to buy {0}", numLots);
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
					//MyPortfolioData["CRTDR"] = crtdr;
	                buyOrder.Send(numLots);
	            }
//				else if (GetSignalDown(crtdr, rsi, trend))
//	            {
//	           		Output.WriteLine("SHORT on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLong.Value, xAverageShort.Value);
//					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = -Bars.CloseValue;
//					sellShortOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
//	            }
				else
				{
	           		Log("--- NOP ---");
				}
			}
			else if(StrategyInfo.MarketPosition > 0)
			{
				// LONG
				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(StrategyInfo.MarketPosition) * StopLossLevel);
				
				// close non-profitable positions straight away because we need to have picked a loser and need to free up money for new deals
				if(CurrentPosition.OpenProfit < 0)
				{
					GenerateExitOnClose();
				}
				else if (this.BarsSinceEntry() > 2 && !GetSignalUp(crtdr, rsi, trend))
				{
	           		Log("!!!SELL1!!!");
					sellOrder.Send("SELL1");
				}
				else if(rsi > (trend == Trend.Down ? RsiSellLevelDown : (trend == Trend.Up ? RsiSellLevelUp : RsiSellLevelFlat)))
				{
	           		Log("!!!SELL2!!!", Bars.TimeValue);
					sellOrder.Send("SELL2");
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
		
		protected override void OnBrokerPositionChange()
		{
			Log("MarketPositionAtBroker={0}, AvgEntryPriceAtBroker={1}", StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker);
		}
	}
}