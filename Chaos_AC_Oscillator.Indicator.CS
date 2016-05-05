using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Chaos_AC_Oscillator : IndicatorObject
    {
        private VariableSeries<Double> m_ao;

        private VariableSeries<Double> m_ac;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Chaos_AC_Oscillator(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_ao = new VariableSeries<Double>(this);
            m_ac = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("+AC", EPlotShapes.Histogram, Color.Lime,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("-AC", EPlotShapes.Histogram, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        private ISeries<double> m_avg_price;
        protected override void StartCalc(){
            m_avg_price = new Lambda<Double>(_bb => ((Bars.High[_bb] + Bars.Low[_bb])/2));
        }

        protected override void CalcBar(){
            m_ao.Value = m_avg_price.Average(5) - m_avg_price.Average(34);
            m_ac.Value = m_ao.Value - m_ao.Average(5);
            if (Bars.CurrentBar >= 5){
                if (PublicFunctions.DoubleGreater(m_ac.Value, m_ac[1])){
                    Plot1.Set(0, m_ac.Value);
                }
                else{
                    Plot2.Set(0, m_ac.Value);
                }
            }
        }
    }
}