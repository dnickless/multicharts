using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Ease_of_Movement : IndicatorObject
    {
        private NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_eom;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Ease_of_Movement(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Magenta;
            upcolor = Color.Cyan;
            colornormlength = 14;
            smoothinglength = 14;
        }

        [Input]
        public int smoothinglength { get; set; }

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
            m_eom = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("EOM", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_normgradientcolor1.dataseriesvalue = m_eom;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (Int32)Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_eom.Value = Bars.CalcEaseOfMovementSeries().Average(smoothinglength);
            Plot1.Set(0, m_eom.Value);
            Plot2.Set(0, 0);
            if ((Color.Empty != upcolor && Color.Empty != dncolor)){
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
            if (this.CrossesOver(m_eom, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_eom, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}