using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Chaos_Awesome_Oscillator : IndicatorObject
    {
        private VariableSeries<Double> m_ao;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Chaos_Awesome_Oscillator(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_ao = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("+AO", EPlotShapes.Histogram,
                                           Color.LimeGreen, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("-AO", EPlotShapes.Histogram, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Empty,
                                           Color.Empty, 0, 0, true));
        }

        private ISeries<double> m_avg_price;
        protected override void StartCalc(){
            m_avg_price = new Lambda<Double>(_bb => ((Bars.High[_bb] + Bars.Low[_bb]) / 2));
        }

        protected override void CalcBar(){
            m_ao.Value = m_avg_price.Average(5) - m_avg_price.Average(34);
            Plot3.Set(0, 0);
            
            if (Bars.CurrentBar < 1)
                return;

            if (PublicFunctions.DoubleGreater(m_ao.Value, m_ao[1])){
                Plot1.Set(0, m_ao.Value);
            }
            else{
                Plot2.Set(0, m_ao.Value);
            }
        }
    }
}