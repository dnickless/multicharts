using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Volume_Brkout : IndicatorObject
    {
        private double m_brkoutfactor;

        private IPlotObject Plot1;

        public Volume_Brkout(object ctx) :
            base(ctx){
            brkoutpct = 50;
            avglength = 50;
        }

        [Input]
        public int avglength { get; set; }

        [Input]
        public double brkoutpct { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("VolBrkOut", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
        }

        private ISeries<double> Volume { get; set; }
        protected override void StartCalc(){
            m_brkoutfactor = 1 + brkoutpct*0.01;
            Volume = Bars.TrueVolume();
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreaterEquals(
                Volume[0], Volume.Average(avglength)*m_brkoutfactor)
            ){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
        }
    }
}