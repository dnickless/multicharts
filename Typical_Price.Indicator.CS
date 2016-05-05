using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Typical_Price : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_typprice;
        private VariableSeries<Double> m_avg;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Typical_Price(object ctx) :
            base(ctx){
            length = 9;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_typprice = new VariableSeries<Double>(this);
            m_avg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("TypPrice", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Avg", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
        }

        protected override void StartCalc(){
            m_averagefc1.price = m_typprice;
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            m_typprice.Value = Bars.TypicalPrice(0);
            m_avg.Value = m_averagefc1[0];
            Plot1.Set(0, m_typprice.Value);
            Plot2.Set(0, m_avg.Value);
            if (this.CrossesOver(m_typprice, m_avg)){
                Alerts.Alert("Typ Price crossing over its average");
            }
            else{
                if (this.CrossesUnder(m_typprice, m_avg)){
                    Alerts.Alert("Typ Price crossing under its average");
                }
            }
        }
    }
}