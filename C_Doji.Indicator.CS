using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_Doji : IndicatorObject
    {
        private IPlotObject Plot1;

        public C_Doji(object ctx) :
            base(ctx){
            percent = 5;
        }

        [Input]
        public double percent { get; set; }

        protected override void Create()
        {
            Plot1 = AddPlot(new PlotAttributes("Doji", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, true));
        }

        protected override void CalcBar()
        {
            if (Bars.Doji(percent, 0))
            {
                Plot1.Set(0, Bars.Close[0]);
                Alerts.Alert("Doji");
            }
        }
    }
}