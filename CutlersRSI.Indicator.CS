using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class CutlersRSI : IndicatorObject {
		public CutlersRSI(object _ctx):base(_ctx){
			cutlersRSIIndicatorMath = new CutlersRSIIndicatorMath(this);
		}
		
		private IPlotObject plot1;
		
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMath;
		
		protected override void Create() {
			// create variable objects, function objects, plot objects etc.
			plot1 = AddPlot(new PlotAttributes("", EPlotShapes.Line, Color.Red));
		}
		protected override void StartCalc() {
			// assign inputs 
		}
		protected override void CalcBar(){
			// indicator logic 
			plot1.Set(cutlersRSIIndicatorMath.CalcNextValue(Bars.Close[0]));
		}
	}
}