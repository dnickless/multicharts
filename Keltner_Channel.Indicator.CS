using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Keltner_Channel : IndicatorObject
    {
        private ISeries<Double> m_price;

        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_lowerband;

        private VariableSeries<Double> m_upperband;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Keltner_Channel(object ctx) :
            base(ctx){
            numatrs = 1.5;
            length = 20;
        }

        private ISeries<Double> price{
            get { return m_price; }
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double numatrs { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("MidLine", 0, Color.Gray,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            var m_avg = m_averagefc1[0];
            var m_shift = (numatrs* this.AverageTrueRange(length));
            m_upperband.Value = (m_avg + m_shift);
            m_lowerband.Value = (m_avg - m_shift);
            if (((displace >= 0)
                 || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_upperband.Value);
                Plot2.Set(displace, m_lowerband.Value);
                Plot3.Set(displace, m_avg);
                if ((displace <= 0)){
                    if (this.CrossesOver(price, m_upperband)){
                        Alerts.Alert("Price crossing over upper band");
                    }
                    else{
                        if (this.CrossesUnder(price, m_lowerband)){
                            Alerts.Alert("Price crossing under lower band");
                        }
                    }
                }
            }
        }
    }
}