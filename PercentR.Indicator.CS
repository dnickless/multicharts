using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class PercentR : IndicatorObject
    {
        private VariableSeries<Double> m_pctr;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public PercentR(object ctx) :
            base(ctx){
            overbcolor = Color.Red;
            overscolor = Color.Cyan;
            overbought = 80;
            oversold = 20;
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
            m_pctr = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("%R", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_pctr.Value = Bars.PercentR(length);
            Plot1.Set(0, m_pctr.Value);
            Plot2.Set(0, overbought);
            Plot3.Set(0, oversold);
            if (PublicFunctions.DoubleGreater(m_pctr.Value, overbought)){
                Plot1.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(m_pctr.Value, oversold)){
                    Plot1.Colors[0] = overscolor;
                }
            }
            if (this.CrossesOver(m_pctr, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_pctr, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}