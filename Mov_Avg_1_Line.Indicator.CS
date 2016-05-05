using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_1_Line : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_avg;

        private IPlotObject Plot1;

        public Mov_Avg_1_Line(object ctx) :
            base(ctx){
            length = 9;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_avg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Avg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = length;
        }

        protected override void CalcBar(){
            m_avg.Value = m_averagefc1[0];
            if (displace >= 0 || Bars.CurrentBar > Math.Abs(displace)){
                Plot1.Set(displace, m_avg.Value);
                if (displace <= 0){
                    if (this.CrossesOver(price, m_avg)){
                        Alerts.Alert("Price crossing over average");
                    }
                    else{
                        if (this.CrossesUnder(price, m_avg)){
                            Alerts.Alert("Price crossing under average");
                        }
                    }
                }
            }
        }
    }
}