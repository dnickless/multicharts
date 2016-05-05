using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Price_Osc : IndicatorObject
    {
        private PriceOscillator m_priceoscillator1;

        private NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_priceosc;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Price_Osc(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Magenta;
            upcolor = Color.Yellow;
            colornormlength = 14;
            slowlength = 18;
            fastlength = 9;
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
            m_priceoscillator1 = new PriceOscillator(this);
            m_normgradientcolor1 = new NormGradientColor(this);
            m_priceosc = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("PriceOsc", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_priceoscillator1.price = Bars.Close;
            m_priceoscillator1.fastlength = fastlength;
            m_priceoscillator1.slowlength = slowlength;
            m_normgradientcolor1.dataseriesvalue = m_priceosc;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (int) Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_priceosc.Value = m_priceoscillator1[0];
            Plot1.Set(0, m_priceosc.Value);
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
            if (this.CrossesOver(m_priceosc, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_priceosc, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}