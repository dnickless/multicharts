using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Volume_Dn : IndicatorObject
    {
        private IPlotObject Plot1;

        public Volume_Dn(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("VolDn", EPlotShapes.Histogram, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }


        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;
            if ( EResolution.Quarter == resolution ||
                EResolution.Day <= resolution && resolution <= EResolution.Year ) return;

            Plot1.Set(0, Bars.DownTicks[0]);
        }
    }
}