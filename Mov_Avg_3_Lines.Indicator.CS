using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_3_Lines : IndicatorObject
    {
        private AverageFC m_averagefc1;
        private AverageFC m_averagefc2;
        private AverageFC m_averagefc3;

        private VariableSeries<Boolean> m_condition1;
        private VariableSeries<Boolean> m_condition2;

        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private IPlotObject Plot3;

        public Mov_Avg_3_Lines(object ctx) :
            base(ctx){
            slowlength = 18;
            medlength = 9;
            fastlength = 4;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int medlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_averagefc2 = new AverageFC(this);
            m_averagefc3 = new AverageFC(this);
            m_condition1 = new VariableSeries<Boolean>(this);
            m_condition2 = new VariableSeries<Boolean>(this);
            Plot1 =
                AddPlot(new PlotAttributes("FastAvg", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("MedAvg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("SlowAvg", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = fastlength;
            m_averagefc2.price = price;
            m_averagefc2.length = medlength;
            m_averagefc3.price = price;
            m_averagefc3.length = slowlength;
        }


        protected override void CalcBar(){
            var m_fastavg = m_averagefc1[0];
            var m_medavg = m_averagefc2[0];
            var m_slowavg = m_averagefc3[0];
            if (displace >= 0 || Bars.CurrentBar > Math.Abs(displace)){
                Plot1.Set(displace, m_fastavg);
                Plot2.Set(displace, m_medavg);
                Plot3.Set(displace, m_slowavg);
                if ((displace <= 0)){
                    m_condition1.Value = ((PublicFunctions.DoubleGreater(price[0], m_fastavg) &&
                                           PublicFunctions.DoubleGreater(m_fastavg, m_medavg))
                                          && PublicFunctions.DoubleGreater(m_medavg, m_slowavg));
                    if ((m_condition1.Value
                         && (m_condition1[1] == false))){
                        Alerts.Alert("Bullish alert");
                    }
                    else{
                        m_condition2.Value = ((PublicFunctions.DoubleLess(price[0], m_fastavg) &&
                                               PublicFunctions.DoubleLess(m_fastavg, m_medavg))
                                              && PublicFunctions.DoubleLess(m_medavg, m_slowavg));
                        if ((m_condition2.Value
                             && (m_condition2[1] == false))){
                            Alerts.Alert("Bearish alert");
                        }
                    }
                }
            }
        }
    }
}