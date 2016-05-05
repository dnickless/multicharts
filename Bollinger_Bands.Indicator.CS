using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Bollinger_Bands : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_lowerband;
        private VariableSeries<Double> m_upperband;

        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private IPlotObject Plot3;

        public Bollinger_Bands(object ctx) :
            base(ctx){
            numdevsdn = -2;
            numdevsup = 2;
            length = 20;
        }

        private ISeries<double> bollingerprice { get; set; }

        private ISeries<double> testpriceuband { get; set; }

        private ISeries<double> testpricelband { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public double numdevsup { get; set; }

        [Input]
        public double numdevsdn { get; set; }

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
            bollingerprice = Bars.Close;
            testpriceuband = Bars.Close;
            testpricelband = Bars.Close;
            m_averagefc1.price = bollingerprice;
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            var m_avg = m_averagefc1[0];
            var m_sdev = bollingerprice.StandardDeviationCustom(length, 1);
            m_upperband.Value = (m_avg + (numdevsup*m_sdev));
            m_lowerband.Value = (m_avg + (numdevsdn*m_sdev));
            if (((displace >= 0) || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_upperband.Value);
                Plot2.Set(displace, m_lowerband.Value);
                Plot3.Set(displace, m_avg);
                if ((displace <= 0)){
                    if (this.CrossesOver(testpricelband, m_lowerband)){
                        Alerts.Alert("Price crossing over lower price band");
                    }
                    else{
                        if (this.CrossesUnder(testpriceuband, m_upperband)){
                            Alerts.Alert("Price crossing under upper price band");
                        }
                    }
                }
            }
        }
    }
}