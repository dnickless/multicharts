using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class CCI_Average : IndicatorObject
    {
        private VariableSeries<Double> m_ccivalue;

        private VariableSeries<Double> m_cciavg;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public CCI_Average(object ctx) :
            base(ctx){
            overbought = 100;
            oversold = (-1*100);
            cciavglength = 9;
            ccilength = 14;
        }

        [Input]
        public int ccilength { get; set; }

        [Input]
        public int cciavglength { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        protected override void Create(){
            m_ccivalue = new VariableSeries<Double>(this);
            m_cciavg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("CCI", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("CCIAvg", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_ccivalue.Value = Bars.CCI(ccilength);
            m_cciavg.Value = m_ccivalue.Average(cciavglength);
            Plot1.Set(0, m_ccivalue.Value);
            Plot2.Set(0, m_cciavg.Value);
            Plot3.Set(0, overbought);
            Plot4.Set(0, oversold);
            if (this.CrossesOver(m_cciavg, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_cciavg, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}