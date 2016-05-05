using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Outside_Bar : IndicatorObject
    {
        private IPlotObject Plot1;

        public Outside_Bar(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Outside", EPlotShapes.Point,
                                           Color.Magenta, Color.Empty, 4,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            if ((PublicFunctions.DoubleGreater(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low[1]))){
                Plot1.Set(0, Bars.Close[0]);
                Alerts.Alert();
            }
        }
    }
}