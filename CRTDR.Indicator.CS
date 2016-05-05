using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class CRTDR : IndicatorObject {
		public CRTDR(object _ctx):base(_ctx){}
		private IPlotObject plot1;
		
		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			plot1 = AddPlot(new PlotAttributes("CRTDR", EPlotShapes.Line, Color.Red));
		}
		
		protected override void CalcBar(){
			// indicator logic 
			var high = Bars.High[0];
			var low = Bars.Low[0];
			var close = Bars.Close[0];
			
			plot1.Set(CRTDRIndicatorMath.CalcNextValue(high, low, close));
		}
	}
}