using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Next_3rd_Friday : IndicatorObject
    {
        
        private IPlotObject Plot1;

        public Next_3rd_Friday(object ctx) :
            base(ctx){
            series = 1;
        }

        [Input]
        public int series { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Next3F", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        
        protected override void CalcBar(){
            Plot1.Set(0, Bars.Next3rdFriday(series, 0));
        }
    }
}