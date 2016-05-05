using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Accum_Swing_Index : IndicatorObject
    {
        private AccumSwingIndex m_accumswingindex1;

        private IPlotObject Plot1;

        public Accum_Swing_Index(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_accumswingindex1 = new AccumSwingIndex(this);
            Plot1 =
                AddPlot(new PlotAttributes("ASX", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_accumswingindex1[0]);
        }
    }
}