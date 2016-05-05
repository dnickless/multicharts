using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Swing_Index : IndicatorObject
    {
        private NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_swingx;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Swing_Index(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            upcolor = Color.Yellow;
            dncolor = Color.Cyan;
            colornormlength = 14;
        }

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
            m_swingx = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("SwingX", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_normgradientcolor1.dataseriesvalue = m_swingx;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (Int32)Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_swingx.Value = SwingIndex.Calc(Bars, 0);
            Plot1.Set(0, m_swingx.Value);
            Plot2.Set(0, 0);
            if (!upcolor.IsEmpty && !dncolor.IsEmpty){
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
            if (this.CrossesOver(m_swingx, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_swingx, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}