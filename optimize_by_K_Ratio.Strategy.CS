using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy {
	public class optimize_by_K_Ratio : SignalObject {
		private K_RatioMath m_KRatio;
		//private IPlotObject kRatioPlot;
		
		public optimize_by_K_Ratio(object ctx) : 
			base(ctx) {
		}

	    [Input]
	    public int NumberOfDataBarsInCalendarYear{ get; set; }
		
		protected override void Create() {
			//kRatioPlot = AddPlot(new PlotAttributes("K-Ratio", EPlotShapes.Line, Color.ForestGreen, Color.Black, 2, EPlotStyle.Solid, true));
			m_KRatio = new K_RatioMath(this);
		}
		
		protected override void StartCalc() {
            m_KRatio.NumberOfDataBarsInCalendarYear = NumberOfDataBarsInCalendarYear;
		}
		
		protected override void CalcBar() {
			var result = m_KRatio.Value;
			if (Bars.LastBarOnChart)
			{
            	CustomFitnessValue = result;
			}
		}
	}
}