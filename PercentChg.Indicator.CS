using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class PercentChg : IndicatorObject
    {
        private VariableSeries<Double> m_pctchg;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public PercentChg(object ctx) :
            base(ctx){
            dncolor = Color.Red;
            upcolor = Color.Cyan;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double hialert { get; set; }

        [Input]
        public double loalert { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_pctchg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("%Chg", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("HiAlert", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("LoAlert", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_pctchg.Value = 100*Bars.Close.PercentChange(length, 0);
            Plot1.Set(0, m_pctchg.Value);
            Plot2.Set(0, hialert);
            Plot3.Set(0, loalert);
            if (PublicFunctions.DoubleGreater(m_pctchg.Value, hialert)){
                Plot1.Colors[0] = upcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(m_pctchg.Value, loalert)){
                    Plot1.Colors[0] = dncolor;
                }
            }
            if (this.CrossesOver(m_pctchg, hialert)){
                Alerts.Alert("Bullish alert");
            }
            else{
                if (this.CrossesUnder(m_pctchg, loalert)){
                    Alerts.Alert("Bearish alert");
                }
            }
        }
    }
}