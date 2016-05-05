using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Detrended_Price_Osc : IndicatorObject
    {
        private NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_dpo;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Detrended_Price_Osc(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Magenta;
            upcolor = Color.Yellow;
            colornormlength = 14;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        [Input]
        public Color gridforegroundcolor { get; set; }

        protected override void Create(){
            m_normgradientcolor1 = new NormGradientColor(this);
            m_dpo = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("DPO", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_normgradientcolor1.dataseriesvalue = m_dpo;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (int) Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_dpo.Value = Bars.Close.Detrend(length);
            Plot1.Set(0, m_dpo.Value);
            Plot2.Set(0, 0);
            {
                var m_colorlevel = m_normgradientcolor1[0];
                if ((m_applicationtype == 1)){
                    Plot1.Colors[0] = m_colorlevel;
                }
                else{
                    if (m_applicationtype > 1){
                        Plot1.Colors[0] = gridforegroundcolor;
                        Plot1.BGColor = m_colorlevel;
                    }
                }
            }
        }
    }
}