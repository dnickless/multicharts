using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Open_Interest : IndicatorObject
    {
        private IPlotObject Plot1;

        public Open_Interest(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("OpenInt", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
        }


        protected override void CalcBar(){
            if ((Bars.OpenInt[0] != 0)){
                Plot1.Set(0, Bars.OpenInt[0]);
            }
        }
    }
}