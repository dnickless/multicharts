using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Comm_Channel_Index : IndicatorObject
    {
        private VariableSeries<Double> m_ccivalue;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Comm_Channel_Index(object ctx) :
            base(ctx){
            overbcolor = Color.Red;
            overscolor = Color.Cyan;
            length = 14;
            oversold = (-1*100);
            overbought = 100;
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
            m_ccivalue = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("CCI", 0, ColorTranslator.FromWin32(10789024),
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_ccivalue.Value = Bars.CCI(length);
            Plot1.Set(0, m_ccivalue.Value);
            Plot2.Set(0, overbought);
            Plot3.Set(0, oversold);
            if (PublicFunctions.DoubleGreater(m_ccivalue.Value, overbought)){
                Plot1.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(m_ccivalue.Value, oversold)){
                    Plot1.Colors[0] = overscolor;
                }
            }
            if (this.CrossesOver(m_ccivalue, oversold)){
                Alerts.Alert("Indicator exiting oversold zone");
            }
            else{
                if (this.CrossesUnder(m_ccivalue, overbought)){
                    Alerts.Alert("Indicator exiting overbought zone");
                }
            }
        }
    }
}