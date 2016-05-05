using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class AdvanceDecl_Line : IndicatorObject
    {
        private AdvanceDeclineDiff m_advancedeclinediff1;

        private IPlotObject Plot1;

        public AdvanceDecl_Line(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_advancedeclinediff1 = new AdvanceDeclineDiff(this);
            Plot1 =
                AddPlot(new PlotAttributes("A/DLine", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_advancedeclinediff1.advissues = Bars.Close;
            m_advancedeclinediff1.decissues = BarsOfData(2).Close;
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_advancedeclinediff1[0]);
        }
    }
}