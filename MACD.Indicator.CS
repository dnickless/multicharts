using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class MACD : IndicatorObject
    {
        private Function.MACD m_macd1;

        private Function.XAverage m_xaverage1;

        private VariableSeries<Double> m_mymacd;

        private VariableSeries<Double> m_macddiff;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public MACD(object ctx) :
            base(ctx){
            macdlength = 9;
            slowlength = 26;
            fastlength = 12;
        }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public int macdlength { get; set; }

        protected override void Create(){
            m_macd1 = new Function.MACD(this);
            m_xaverage1 = new Function.XAverage(this);
            m_mymacd = new VariableSeries<Double>(this);
            m_macddiff = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MACD", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("MACDAvg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("MACDDiff", EPlotShapes.Histogram,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_macd1.Price = Bars.Close;
            m_macd1.FastLength = fastlength;
            m_macd1.SlowLength = slowlength;
            m_xaverage1.Price = m_mymacd;
            m_xaverage1.Length = macdlength;
            m_mymacd.DefaultValue = 0;
            m_macddiff.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_mymacd.Value = m_macd1[0];
            var m_macdavg = m_xaverage1[0];
            m_macddiff.Value = m_mymacd.Value - m_macdavg;
            Plot1.Set(0, m_mymacd.Value);
            Plot2.Set(0, m_macdavg);
            Plot3.Set(0, m_macddiff.Value);
            Plot4.Set(0, 0);
            if (this.CrossesOver(m_macddiff, 0)){
                Alerts.Alert("Bullish alert");
            }
            else{
                if (this.CrossesUnder(m_macddiff, 0)){
                    Alerts.Alert("Bearish alert");
                }
            }
        }
    }
}