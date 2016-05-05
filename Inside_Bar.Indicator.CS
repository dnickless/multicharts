using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Inside_Bar : IndicatorObject
    {
        private IPlotObject Plot1;

        public Inside_Bar(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Inside", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
        }


        protected override void CalcBar(){
            if ((PublicFunctions.DoubleLess(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleGreater(Bars.Low[0], Bars.Low[1]))){
                Plot1.Set(0, Bars.Close[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}