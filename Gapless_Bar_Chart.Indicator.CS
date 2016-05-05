using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Gapless_Bar_Chart : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Gapless_Bar_Chart(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("TrueHigh", EPlotShapes.BarHigh,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("TrueLow", EPlotShapes.BarLow,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("Open", EPlotShapes.LeftTick,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("Close", EPlotShapes.RightTick,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            Plot1.Set(0, _TrueHigh.TrueHigh(Bars, 0));
            Plot2.Set(0, _TrueLow.TrueLow(Bars, 0));
            Plot3.Set(0, Bars.Open[0]);
            Plot4.Set(0, Bars.Close[0]);
        }
    }
}