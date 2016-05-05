using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Count_Criteria_PB : IndicatorObject
    {
        private ISeries<Boolean> m_criteria;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Count_Criteria_PB(object ctx) :
            base(ctx){
            occur = 7;
            length = 14;
        }

        private ISeries<Boolean> criteria{
            get { return m_criteria; }
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int occur { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_criteria =
                new Lambda<Boolean>(
                    _bb => PublicFunctions.DoubleGreater(Bars.Close[_bb], Bars.High[(1 + _bb)]));
        }


        protected override void CalcBar(){
            if (criteria.CountIF(length) >= occur){
                Plot1.Set(0, Bars.High[0]);
                Plot2.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}