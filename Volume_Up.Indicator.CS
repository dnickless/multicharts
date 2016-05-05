using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Volume_Up : IndicatorObject
    {
        private IPlotObject Plot1;

        public Volume_Up(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("VolUp", EPlotShapes.Histogram,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            EResolution resolution = Bars.Info.Resolution.Type;
            if (EResolution.Quarter == resolution ||
                EResolution.Day <= resolution && resolution <= EResolution.Year) return;

            Plot1.Set(0, Bars.UpTicks[0]);
        }
    }
}