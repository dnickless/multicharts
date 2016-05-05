using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class HPI : IndicatorObject
    {
        private Function.HPI m_hpi1;

        private NormGradientColor m_normgradientcolor1;

        private VariableObject<Int32> m_applicationtype;

        private VariableSeries<Double> m_hpivalue;

        private Color m_colorlevel;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public HPI(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Cyan;
            upcolor = Color.Yellow;
            colornormlength = 14;
            alertlength = 14;
            smoothingfactor = 0.133;
            onecent = 100;
        }

        [Input]
        public double onecent { get; set; }

        [Input]
        public double smoothingfactor { get; set; }

        [Input]
        public int alertlength { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        [Input]
        public Color gridforegroundcolor { get; set; }

        protected override void Create(){
            m_hpi1 = new Function.HPI(this);
            m_normgradientcolor1 = new Function.NormGradientColor(this);
            m_applicationtype = new VariableObject<Int32>(this);
            m_hpivalue = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("HPI", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_hpi1.onecent = onecent;
            m_hpi1.smfactor = smoothingfactor;
            m_normgradientcolor1.dataseriesvalue = m_hpivalue;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype.DefaultValue = 0;
            m_hpivalue.DefaultValue = 0;
            m_colorlevel = Color.Empty;
        }


        protected override void CalcBar(){
            if (Bars.CurrentBar == 1){
                m_applicationtype.Value = (Int32) Environment.ApplicationCode;
            }
            m_hpivalue.Value = m_hpi1[0]*1E-05;
            Plot1.Set(0, m_hpivalue.Value);
            Plot2.Set(0, 0);
            if (Color.Empty != upcolor && Color.Empty != dncolor){
                m_colorlevel = m_normgradientcolor1[0];
                if (m_applicationtype.Value == 1){
                    Plot1.Colors[0] = m_colorlevel;
                }
                else{
                    if (m_applicationtype.Value > 1){
                        Plot1.Colors[0] = gridforegroundcolor;
                        Plot1.BGColor = m_colorlevel;
                    }
                }
            }
            if (Bars.Close.LowestBar(alertlength) == 0
                 && PublicFunctions.DoubleGreater(m_hpivalue.LowestBar(alertlength), 0))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (Bars.Close.HighestBar(alertlength) == 0
                     && PublicFunctions.DoubleGreater(m_hpivalue.HighestBar(alertlength), 0))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}