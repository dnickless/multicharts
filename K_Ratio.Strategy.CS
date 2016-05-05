using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy {
	public class K_Ratio : PortfolioSignalObject {
		public K_Ratio(object _ctx):base(_ctx){}
		//private IOrderPriced buyOrder;
		private IOrderMarket buyOrderMarket;
		private IOrderMarket sellOrderCutLoss;
		private IOrderMarket sellOrderRsi;
		
		private K_RatioBarsMath kRatioBarsMathShort;
		private K_RatioBarsMath kRatioBarsMathLong;
		private RSI rsi;
		
		[Input]
        public int RsiLimitUp {get; set;}
		
		[Input]
        public int RsiLimitDown {get; set;}
		
        [Input]
        public int Rsi {get; set;}
		
		[Input]
		public int BarsToExitAfterIfNoProfit { get; set; }
		
        [Input]
        public int KRatioShortLength {get; set;}
        
        [Input]
        public double KRatioShortLimit {get; set;}
		
        [Input]
        public int KRatioLongLength {get; set;}
        
        [Input]
        public double KRatioLongLimit {get; set;}
        
		protected override void Create() {
			//buyOrder = OrderCreator.Limit(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
			buyOrderMarket = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy)); // we need "Default" in order for MM Signal to work
            sellOrderRsi = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "RSI Exit", EOrderAction.Sell, OrderExit.FromAll));
            sellOrderCutLoss = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "Cut Loss Exit", EOrderAction.Sell, OrderExit.FromAll));
			rsi = new RSI(this);
			kRatioBarsMathShort = new K_RatioBarsMath(this);
			kRatioBarsMathLong = new K_RatioBarsMath(this);
		}
		
		protected override void StartCalc() {
			rsi.length = Rsi;
			rsi.price = Bars.Close;
			kRatioBarsMathShort.NumberOfDataBarsInCalendarYear = 1;
			kRatioBarsMathShort.Length = KRatioShortLength;
			kRatioBarsMathShort.Price = Bars.Close;
			kRatioBarsMathLong.NumberOfDataBarsInCalendarYear = 1;
			kRatioBarsMathLong.Length = KRatioLongLength;
			kRatioBarsMathLong.Price = Bars.Close;
		}
		
		int i = 0;
		
		private bool doReinvestment = false;
		
		static double maxShort = 0;
		static double maxLong = 0;
		static double minShort = 0;
		static double minLong = 0;
		protected override void CalcBar()
		{
			// todo: re-enable for MM signal to work
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
            if (StrategyInfo.MarketPosition == 0)
			{
//				var signals = PortfolioStrategies.Select(s => s.Signals[0]).Where(s => s.Positions.Count > 0).ToList();
//				var numNegPos = (double)signals.Count(s => s.CurrentPosition.OpenProfit < 0);
//				var numPosPos = (double)signals.Count(s => s.CurrentPosition.OpenProfit > 0);
//				if(numNegPos > 0 && numNegPos / (numPosPos + numNegPos) >= 0.5)
//					return;

				if(rsi.Value < RsiLimitDown && kRatioBarsMathShort.Value > KRatioShortLimit && kRatioBarsMathLong.Value < KRatioLongLimit)
				{
					//MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
					//buyOrder.Send(Bars.CloseValue, Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 3.0 / Bars.CloseValue));
                    
					// todo: re-enable for MM signal to work
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
					buyOrderMarket.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 100.0 / Bars.CloseValue));
				}
			}
			else
			{
				if(rsi.Value > RsiLimitUp)
					sellOrderRsi.Send();
				
//				if(this.BarsSinceEntry() >= BarsToExitAfterIfNoProfit && CurrentPosition.OpenProfit < 0)
//					sellOrderCutLoss.Send();
			}
		}
	}
}