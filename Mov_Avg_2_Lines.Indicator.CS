using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_2_Lines : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private AverageFC m_averagefc2;

        private VariableSeries<Double> m_fastavg;

        private VariableSeries<Double> m_slowavg;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Mov_Avg_2_Lines(object ctx) :
            base(ctx){
            slowlength = 18;
            fastlength = 9;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_averagefc2 = new AverageFC(this);
            m_fastavg = new VariableSeries<Double>(this);
            m_slowavg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("FastAvg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("SlowAvg", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = fastlength;
            m_averagefc2.price = price;
            m_averagefc2.length = slowlength;
        }


        protected override void CalcBar(){
            m_fastavg.Value = m_averagefc1[0];
            m_slowavg.Value = m_averagefc2[0];
            if (displace >= 0 || Bars.CurrentBar > Math.Abs(displace)){
                Plot1.Set(displace, m_fastavg.Value);
                Plot2.Set(displace, m_slowavg.Value);
                if ((displace <= 0)){
                    if (this.CrossesOver(m_fastavg, m_slowavg)){
                        Alerts.Alert("Bullish alert");
                    }
                    else{
                        if (this.CrossesUnder(m_fastavg, m_slowavg)){
                            Alerts.Alert("Bearish alert");
                        }
                    }
                }
            }
        }
    }
}