using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Envelopes : IndicatorObject
    {
        private AverageFC m_averagefc1;
        private AverageFC m_averagefc2;

        private VariableSeries<Double> m_lowerband;
        private VariableSeries<Double> m_upperband;

        private IPlotObject Plot1;
        private IPlotObject Plot2;

        public Mov_Avg_Envelopes(object ctx) :
            base(ctx){
            pctbelow = 1;
            pctabove = 1;
            length = 20;
        }

        private ISeries<double> priceh { get; set; }

        private ISeries<double> pricel { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public double pctabove { get; set; }

        [Input]
        public double pctbelow { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_averagefc2 = new AverageFC(this);
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        private double m_pctbelowfactor;
        private double m_pctabovefactor;

        protected override void StartCalc(){
            priceh = pricel = Bars.Close;

            m_averagefc1.price = pricel;
            m_averagefc1.length = length;
            m_averagefc2.price = priceh;
            m_averagefc2.length = length;
            m_lowerband.DefaultValue = 0;
            m_upperband.DefaultValue = 0;
            m_pctbelowfactor = (1 - (pctbelow*0.01));
            m_pctabovefactor = (1 + (pctabove*0.01));
        }


        protected override void CalcBar(){
            m_lowerband.Value = (m_averagefc1[0]*m_pctbelowfactor);
            m_upperband.Value = (m_averagefc2[0]*m_pctabovefactor);
            if (((displace >= 0)
                 || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_upperband.Value);
                Plot2.Set(displace, m_lowerband.Value);
                if ((displace <= 0)){
                    if (this.CrossesOver(pricel, m_lowerband)){
                        Alerts.Alert("Price crossing over lower price band");
                    }
                    else{
                        if (this.CrossesUnder(priceh, m_upperband)){
                            Alerts.Alert("Price crossing under upper price band");
                        }
                    }
                }
            }
        }
    }
}