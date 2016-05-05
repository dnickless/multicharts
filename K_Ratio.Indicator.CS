using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class K_Ratio : IndicatorObject {
		public K_Ratio(object _ctx):base(_ctx){
			kRatioBarsMath = new K_RatioBarsMath(this);
		}
		
		private IPlotObject plot1;
		
		private K_RatioBarsMath kRatioBarsMath;
		
        [Input]
        public int Length {get; set;}
        
		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
		}
		protected override void StartCalc() {
			kRatioBarsMath.Price = Bars.Close;
			kRatioBarsMath.Length = Length;
			kRatioBarsMath.NumberOfDataBarsInCalendarYear = 1;
		}
		protected override void CalcBar(){
			// indicator logic 
			plot1.Set(kRatioBarsMath.Value);
		}
	}
}