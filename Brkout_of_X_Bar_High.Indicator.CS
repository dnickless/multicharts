using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Brkout_of_X_Bar_High : IndicatorObject
    {
        private IPlotObject Plot1;

        public Brkout_of_X_Bar_High(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("BrkoutHi", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.High[0], Bars.High.Highest(length, 1))){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
        }
    }
}