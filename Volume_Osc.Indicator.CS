using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Volume_Osc : IndicatorObject
    {
        private VolumeOsc m_volumeosc1;

        private NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_volosc;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Volume_Osc(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Magenta;
            upcolor = Color.Yellow;
            colornormlength = 14;
            slowlength = 28;
            fastlength = 14;
        }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        [Input]
        public Color gridforegroundcolor { get; set; }

        protected override void Create(){
            m_volumeosc1 = new VolumeOsc(this);
            m_normgradientcolor1 = new NormGradientColor(this);
            m_volosc = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("VolOsc", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_volumeosc1.fastlength = fastlength;
            m_volumeosc1.slowlength = slowlength;
            m_normgradientcolor1.dataseriesvalue = m_volosc;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (Int32)Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_volosc.Value = m_volumeosc1[0];
            Plot1.Set(0, m_volosc.Value);
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
            if (this.CrossesOver(m_volosc, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_volosc, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}