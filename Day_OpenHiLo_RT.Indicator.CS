using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Day_OpenHiLo_RT : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Day_OpenHiLo_RT(object ctx) :
            base(ctx){
            plotopen = true;
        }

        [Input]
        public bool plotopen { get; set; }

        [Input]
        public bool plotprevclose { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("High", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Low", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("Open", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("YestClose", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            if ((Environment.IsRealTimeCalc)){
                var m_dhigh = Bars.StatusLine.High;
                var m_dlow = Bars.StatusLine.Low;
                Plot1.Set(0, m_dhigh);
                Plot2.Set(0, m_dlow);
                if (plotopen){
                    var m_dopen = Bars.StatusLine.Open;
                    Plot3.Set(0, m_dopen);
                }
                if (plotprevclose){
                    var m_yestclose = Bars.StatusLine.PrevClose;
                    Plot4.Set(0, m_yestclose);
                }
            }
        }
    }
}