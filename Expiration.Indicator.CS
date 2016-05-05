using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Expiration : IndicatorObject
    {
        
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Expiration(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Magenta, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Magenta, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
        }


        protected override void CalcBar(){
            if (Bars.Next3rdFriday(1, 1) == 1){
                Plot1.Set(0, Bars.High[0]);
                Plot2.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
        }
    }
}