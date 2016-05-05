using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Parabolic_SAR : IndicatorObject
    {
        private ParabolicSAR m_parabolicsar1;

        private VariableObject<Double> m_oparcl;

        private VariableObject<Double> m_oparop;

        private VariableObject<int> m_oposition;

        private VariableObject<int> m_otransition;

        private IPlotObject Plot1;

        public Parabolic_SAR(object ctx) :
            base(ctx){
            afstep = 0.02;
            aflimit = 0.2;
        }

        [Input]
        public double afstep { get; set; }

        [Input]
        public double aflimit { get; set; }

        protected override void Create(){
            m_parabolicsar1 = new ParabolicSAR(this);
            m_oparcl = new VariableObject<Double>(this);
            m_oparop = new VariableObject<Double>(this);
            m_oposition = new VariableObject<int>(this);
            m_otransition = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("ParCl", EPlotShapes.Point, Color.Blue,
                                           Color.Empty, 1, 0, true));
        }

        protected override void StartCalc(){
            m_parabolicsar1.afstep = afstep;
            m_parabolicsar1.aflimit = aflimit;
            m_parabolicsar1.oparcl = m_oparcl;
            m_parabolicsar1.oparop = m_oparop;
            m_parabolicsar1.oposition = m_oposition;
            m_parabolicsar1.otransition = m_otransition;
        }


        protected override void CalcBar(){
            m_parabolicsar1.Call();
            Plot1.Set(0, m_oparcl.Value);
            if ((m_otransition.Value == 1)){
                Alerts.Alert("Bullish reversal");
            }
            else{
                if ((m_otransition.Value
                     == (-1))){
                    Alerts.Alert("Bearish reversal");
                }
            }
        }
    }
}