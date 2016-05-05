using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Weighted_Close : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_wtdclose;

        private VariableSeries<Double> m_avg;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Weighted_Close(object ctx) :
            base(ctx){
            length = 9;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_wtdclose = new VariableSeries<Double>(this);
            m_avg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("WtdClose", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Avg", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_averagefc1.price = new Lambda<double>(bb=>Bars.WeightedClose(bb));
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            m_wtdclose.Value = Bars.WeightedClose(0);
            m_avg.Value = m_averagefc1[0];
            Plot1.Set(0, m_wtdclose.Value);
            Plot2.Set(0, m_avg.Value);
            if (this.CrossesOver(m_wtdclose, m_avg)){
                Alerts.Alert("Wtd Close crossing over its average");
            }
            else{
                if (this.CrossesUnder(m_wtdclose, m_avg)){
                    Alerts.Alert("Wtd Close crossing under its average");
                }
            }
        }
    }
}