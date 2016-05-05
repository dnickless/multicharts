using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class RSI : IndicatorObject
    {
        private Function.RSI m_rsi1;

        private VariableSeries<Double> m_myrsi;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public RSI(object ctx) :
            base(ctx){
            overbcolor = Color.Red;
            overscolor = Color.Cyan;
            overbought = 70;
            oversold = 30;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        [Input]
        public Color overscolor { get; set; }

        [Input]
        public Color overbcolor { get; set; }

        protected override void Create(){
            m_rsi1 = new Function.RSI(this);
            m_myrsi = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("RSI", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_rsi1.price = Bars.Close;
            m_rsi1.length = length;
        }


        protected override void CalcBar(){
            m_myrsi.Value = m_rsi1[0];
            Plot1.Set(0, m_myrsi.Value);
            Plot2.Set(0, overbought);
            Plot3.Set(0, oversold);
            if (PublicFunctions.DoubleGreater(m_myrsi.Value, overbought)){
                Plot1.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(m_myrsi.Value, oversold)){
                    Plot1.Colors[0] = overscolor;
                }
            }
            if (this.CrossesOver(m_myrsi, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_myrsi, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}