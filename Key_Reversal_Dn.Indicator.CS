using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Key_Reversal_Dn : IndicatorObject
    {
        private IPlotObject Plot1;

        public Key_Reversal_Dn(object ctx) :
            base(ctx){
            length = 1;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("KeyRevDn", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }


        protected override void CalcBar(){
            if ((PublicFunctions.DoubleGreater(Bars.High[0], Bars.High.Highest(length, 1)) &&
                 PublicFunctions.DoubleLess(Bars.Close[0], Bars.Close[1]))){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}