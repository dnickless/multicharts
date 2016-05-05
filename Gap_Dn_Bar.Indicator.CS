using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Gap_Dn_Bar : IndicatorObject
    {
        private IPlotObject Plot1;

        public Gap_Dn_Bar(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("GapDn", EPlotShapes.Point, Color.Blue,
                                           Color.Empty, 4, 0, true));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.High[0], Bars.Low[1])){
                Plot1.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}