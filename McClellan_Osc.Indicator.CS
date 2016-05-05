using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class McClellan_Osc : IndicatorObject
    {
        private McClellanOsc m_mcclellanosc1;

        private VariableSeries<Double> m_mcclosc;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public McClellan_Osc(object ctx) :
            base(ctx){
            overbought = 70;
            oversold = (-1*70);
            slowlength = 39;
            fastlength = 19;
        }

        private ISeries<double> advissues { get; set; }

        private ISeries<double> decissues { get; set; }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        protected override void Create(){
            m_mcclellanosc1 = new McClellanOsc(this);
            m_mcclosc = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("McClOsc", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            advissues = Bars.Close;
            decissues = BarsOfData(2).Close;
            m_mcclellanosc1.advissues = advissues;
            m_mcclellanosc1.decissues = decissues;
            m_mcclellanosc1.fastlength = fastlength;
            m_mcclellanosc1.slowlength = slowlength;
        }


        protected override void CalcBar(){
            m_mcclosc.Value = m_mcclellanosc1[0];
            Plot1.Set(0, m_mcclosc.Value);
            Plot2.Set(0, overbought);
            Plot3.Set(0, oversold);
            if (this.CrossesOver(m_mcclosc, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_mcclosc, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}