using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Momentum : IndicatorObject
    {
        private ISeries<Double> m_price;

        private Function.NormGradientColor m_normgradientcolor1;

        private int m_applicationtype;

        private VariableSeries<Double> m_mom;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Momentum(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Red;
            upcolor = Color.Yellow;
            colornormlength = 14;
            length = 12;
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
            m_mom = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Momentum", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
            m_normgradientcolor1.dataseriesvalue = m_mom;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (Int32) Environment.ApplicationCode;
        }


        protected override void CalcBar(){
            m_mom.Value = m_price.Momentum(length);
            var m_accel = m_mom.Momentum(1);
            Plot1.Set(0, m_mom.Value);
            Plot2.Set(0, 0);
            if (!upcolor.IsEmpty && !dncolor.IsEmpty){
                var m_colorlevel = m_normgradientcolor1[0];
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
            if (PublicFunctions.DoubleGreater(m_mom.Value, 0) 
                && PublicFunctions.DoubleGreater(m_accel, 0)){
                Alerts.Alert("Indicator positive and increasing");
            }
            else{
                if (PublicFunctions.DoubleLess(m_mom.Value, 0) 
                    && PublicFunctions.DoubleLess(m_accel, 0)){
                    Alerts.Alert("Indicator negative and decreasing");
                }
            }
        }
    }
}