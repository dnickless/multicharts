using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class Test : SignalObject {
		public Test(object _ctx):base(_ctx){}
		private IOrderMarket buy_order;
		XAverageThatWorks x;
		
		protected override void Create() {
			// create variable objects, function objects, order objects etc.
			buy_order = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, EOrderAction.Buy));
			x = new XAverageThatWorks(this);
		}
		protected override void StartCalc() {
			x.Length = 80;
			x.Price = Bars.Close;
			// assign inputs 
		}
		protected override void CalcBar(){
			// strategy logic 
			//buy_order.Send();
			//if(Bars.CurrentBar % 2 == 0)
			{
				Output.WriteLine(x.Value.ToString());
			}
		}
	}
}