using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Brkout_of_X_Bar_Low : IndicatorObject
    {
        private IPlotObject Plot1;

        public Brkout_of_X_Bar_Low(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("BrkoutLo", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low.Lowest(length, 1))){
                Plot1.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
        }
    }
}