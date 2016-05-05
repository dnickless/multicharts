using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Arms_Index_TRIN : IndicatorObject
    {
        private VariableSeries<Double> m_trin;

        private VariableSeries<Double> m_avgtrin;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Arms_Index_TRIN(object ctx) :
            base(ctx){
            overbought = 0.7;
            oversold = 1.25;
            smoothinglength = 4;
        }

        private ISeries<double> advissues { get; set; }

        private ISeries<double> advvol { get; set; }

        private ISeries<double> decissues { get; set; }

        private ISeries<double> decvol { get; set; }

        [Input]
        public int smoothinglength { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        protected override void Create(){
            m_trin = new VariableSeries<Double>(this);
            m_avgtrin = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("ArmsX", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ArmsXAvg", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            advissues = Bars.Close;
            advvol = BarsOfData(2).Close;
            decissues = BarsOfData(3).Close;
            decvol = BarsOfData(4).Close;
        }


        protected override void CalcBar(){
            m_trin.Value = ArmsIndex.Calc(advissues, advvol, decissues, decvol);
            m_avgtrin.Value = m_trin.Average(smoothinglength);
            Plot1.Set(0, m_trin.Value);
            Plot2.Set(0, m_avgtrin.Value);
            Plot3.Set(0, oversold);
            Plot4.Set(0, overbought);
            if (this.CrossesUnder(m_avgtrin, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesOver(m_avgtrin, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}