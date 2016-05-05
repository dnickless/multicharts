using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Key_Reversal_Up : IndicatorObject
    {
        private IPlotObject Plot1;

        public Key_Reversal_Up(object ctx) :
            base(ctx){
            length = 1;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("KeyRevUp", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            if ((PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low.Lowest(length, 1)) &&
                 PublicFunctions.DoubleGreater(Bars.Close[0], Bars.Close[1]))){
                Plot1.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}