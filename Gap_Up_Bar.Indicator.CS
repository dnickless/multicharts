using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Gap_Up_Bar : IndicatorObject
    {
        private IPlotObject Plot1;

        public Gap_Up_Bar(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("GapUp", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.Low[0], Bars.High[1])){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}