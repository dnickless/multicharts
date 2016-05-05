using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class First_Bar_of_Day : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public First_Bar_of_Day(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
        }


        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;

            if ( ( resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter )
                 && (Bars.Time[0].Date != Bars.Time[1].Date))
            {
                Plot1.Set(0, Bars.High[0]);
                Plot2.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
        }
    }
}