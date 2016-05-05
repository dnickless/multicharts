using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Island_Reversal_Dn : IndicatorObject
    {
        private IPlotObject Plot1;

        public Island_Reversal_Dn(object ctx) :
            base(ctx){
            pctrange = 30;
            length = 4;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double pctrange { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("IslandDn", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
        }


        protected override void CalcBar(){
            if ((PublicFunctions.DoubleGreater(Bars.Low[0], Bars.High.Highest(length, 1)) &&
                 PublicFunctions.DoubleLess(Bars.Close[0], Bars.Low[0]
                                                           + Bars.Range()*(pctrange*0.01)))){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}