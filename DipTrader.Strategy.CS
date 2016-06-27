using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
	public class DipTrader : SignalObject
	{
		private IOrderPriced buyOrder;
		private IOrderMarket sellOrder;
		
		private XAverageThatWorks ema3;
		private MACD macd;
		
        [Input]
        public double IndexDown1 {get; set;}
        
        [Input]
        public double IndexDown2 {get; set;}
        
        [Input]
        public double IndexDown3 {get; set;}
        
        [Input]
        public double IndexDown4 {get; set;}
        
        [Input]
        public double XDown {get; set;}
        
		public DipTrader(object _ctx):base(_ctx){}
		
		protected override void Create() {
			// create variable objects, function objects, order objects etc.
			buyOrder = OrderCreator.Limit(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
			sellOrder = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell));
			ema3 = new XAverageThatWorks(this, 1);
			macd = new MACD(this, 1);
		}
		
		protected override void StartCalc()
		{
			ema3.Length = 3;
			ema3.Price = Bars.Close;
			macd.SlowLength = 12;
			macd.FastLength = 26;
			macd.Price = Bars.Close;
		}
		
		protected override void CalcBar()
		{
			if(CurrentPosition.Value != 0) sellOrder.Send();
			
	        int c1down=0;
	        
	        if ((Bars.Close[1] < Bars.Open[1]))c1down++;
	        if ((Bars.CloseValue < Bars.OpenValue))c1down++;
	        if ((Bars.CloseValue < Bars.Close[1]))c1down++;
	        if ((Bars.Close[1] < Bars.Close[3]))c1down++;
	        
	        double multdown=0;
	        
	        if ((c1down <= 1)) multdown=IndexDown1;
	        if ((c1down == 2)) multdown=IndexDown2;
	        if ((c1down == 3)) multdown=IndexDown3;
	        if ((c1down == 4)) multdown=IndexDown4;
	        
			var ema3Value = ema3.Value;
	        var xDown_Up=ema3Value*multdown;
	        var xDown_Down=ema3Value*XDown;
	        
	        var limit =((xDown_Up-xDown_Down)/2)*(Math.Max(Math.Min(macd.Value,-0.5),+0.5))+xDown_Down;
			//Output.WriteLine(Bars.TimeValue.AddDays(1) + " " + Bars.Info.Name + " " + limit);
	        
			buyOrder.Send(limit);
		}
	}
}