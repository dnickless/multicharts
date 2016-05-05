using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class AdvanceDecl_Ratio : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public AdvanceDecl_Ratio(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("A/DRatio", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Baseline", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            Plot1.Set(0, Bars.Close.AdvanceDeclineRatio(BarsOfData(2).Close));
            Plot2.Set(0, 1);
        }
    }
}