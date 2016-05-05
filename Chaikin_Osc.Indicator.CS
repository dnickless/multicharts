using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Chaikin_Osc : IndicatorObject
    {
        private ChaikinOsc m_chaikinosc1;

        private NormGradientColor m_normgradientcolor1;

        private Int32 m_applicationtype;

        private VariableSeries<Double> m_chaikinoscval;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Chaikin_Osc(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Red;
            upcolor = Color.Yellow;
            colornormlength = 14;
            alertlength = 14;
            slowlength = 10;
            fastlength = 3;
        }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

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
            m_chaikinosc1 = new ChaikinOsc(this);
            m_normgradientcolor1 = new NormGradientColor(this);
            m_applicationtype = 0;
            m_chaikinoscval = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("ChaikinOsc", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_chaikinosc1.anyvol = Bars.TrueVolume();
            m_chaikinosc1.shortlength = fastlength;
            m_chaikinosc1.longlength = slowlength;
            m_normgradientcolor1.dataseriesvalue = m_chaikinoscval;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
            m_applicationtype = (Int32) Environment.ApplicationCode;
        }


        protected override void CalcBar()
        {
            m_chaikinoscval.Value = m_chaikinosc1[0];
            Plot1.Set(0, m_chaikinoscval.Value);
            Plot2.Set(0, 0);
            if ((Color.Empty != upcolor && Color.Empty != dncolor))
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
            if (((Bars.Close.LowestBar(alertlength) == 0)
                 && PublicFunctions.DoubleGreater(m_chaikinoscval.LowestBar(alertlength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(alertlength) == 0)
                     && PublicFunctions.DoubleGreater(m_chaikinoscval.HighestBar(alertlength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}