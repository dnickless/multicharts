using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy {
	public class Slope : PortfolioSignalObject {
		public Slope(object _ctx):base(_ctx){}
		private IOrderMarket buyOrderMarket;
		private IOrderMarket sellOrderRsi;
		private IOrderMarket sellOrderStopLoss;
		
		private SlopeMath slopeMath;
		private RSI rsi;
		
		[Input]
        public int RsiLimitUp {get; set;}
		
		[Input]
        public int RsiLimitDown {get; set;}
		
        [Input]
        public int Rsi {get; set;}
		
        [Input]
        public int SlopeLength {get; set;}
		
        [Input]
        public double SlopeLimit {get; set;}
        
		protected override void Create() {
			//buyOrder = OrderCreator.Limit(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
			buyOrderMarket = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy)); // we need "Default" in order for MM Signal to work
            sellOrderRsi = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "RSI Exit", EOrderAction.Sell, OrderExit.FromAll));
            sellOrderStopLoss = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "TimeBarsLX", EOrderAction.Sell, OrderExit.FromAll));

			rsi = new RSI(this);
			slopeMath = new SlopeMath(this);
		}
		
		protected override void StartCalc() {
			rsi.length = Rsi;
			rsi.price = Bars.Close;
			slopeMath.Length = SlopeLength;
			slopeMath.Price = Bars.Close;
		}
		
		private bool doReinvestment = false;
		
		protected override void CalcBar()
		{
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
            if (StrategyInfo.MarketPosition == 0)
			{
				if(slopeMath.Value < SlopeLimit)
				{
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
					buyOrderMarket.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 100.0 / Bars.CloseValue));
				}
			}
			else
			{
				if(Bars.CloseValue > Bars.OpenValue)
					sellOrderRsi.Send();
				
	            if (Bars.CloseValue <= Bars.OpenValue){
	                sellOrderStopLoss.Send();
	            }
			}
		}
	}
}