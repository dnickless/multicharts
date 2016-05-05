using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Bid_And_Ask : IndicatorObject
    {
        private IPlotObject Plot1;
        private IPlotObject Plot2;

        public Bid_And_Ask(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Bid", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Ask", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            if ((Environment.IsRealTimeCalc)){
                Plot1.Set(0, Bars.StatusLine.Bid);
                Plot2.Set(0, Bars.StatusLine.Ask);
            }
        }
    }
}