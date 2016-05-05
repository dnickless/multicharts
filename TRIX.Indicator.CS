using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class TRIX : IndicatorObject
    {
        private Function.TRIX m_trix1;

        private Function.NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_trixvalue;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public TRIX(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Red;
            upcolor = Color.Yellow;
            colornormlength = 14;
            length = 9;
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
            m_trix1 = new Function.TRIX(this);
            m_normgradientcolor1 = new Function.NormGradientColor(this);
            m_trixvalue = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("TRIX", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_trix1.price = Bars.Close;
            m_trix1.length = length;
            m_normgradientcolor1.dataseriesvalue = m_trixvalue;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (int) Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_trixvalue.Value = m_trix1[0];
            Plot1.Set(0, m_trixvalue.Value);
            Plot2.Set(0, 0);
            if (!upcolor.IsEmpty && !dncolor.IsEmpty){
                var m_colorlevel = m_normgradientcolor1.Value;
                if (m_applicationtype == 1){
                    Plot1.Colors[0] = m_colorlevel;
                }
                else{
                    if (m_applicationtype > 1){
                        Plot1.Colors[0] = gridforegroundcolor;
                        Plot1.BGColor = m_colorlevel;
                    }
                }
            }
            if (this.CrossesOver(m_trixvalue, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_trixvalue, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}