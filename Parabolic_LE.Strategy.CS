using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Parabolic_LE : SignalObject
    {
        private ParabolicSAR m_ParabolicSAR;

    
        private VariableObject<Double> m_oparcl;

        private VariableObject<Double> m_oparop;

        private VariableObject<int> m_Oposition;

        private VariableObject<int> m_otransition;

        private IOrderPriced m_ParLE;

        public Parabolic_LE(object ctx) :
            base(ctx)
        {
            AFLimit = 0.2;
            AFStep = 0.02;
        }

        [Input]
        public double AFStep { get; set; }

        [Input]
        public double AFLimit { get; set; }

        protected override void Create(){
            m_ParabolicSAR = new ParabolicSAR(this);
            m_oparcl = new VariableObject<Double>(this);
            m_oparop = new VariableObject<Double>(this);
            m_Oposition = new VariableObject<int>(this);
            m_otransition = new VariableObject<int>(this);
            m_ParLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ParLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            m_ParabolicSAR.afstep = AFStep;
            m_ParabolicSAR.aflimit = AFLimit;
            m_ParabolicSAR.oparcl = m_oparcl;
            m_ParabolicSAR.oparop = m_oparop;
            m_ParabolicSAR.oposition = m_Oposition;
            m_ParabolicSAR.otransition = m_otransition;
        }


        protected override void CalcBar(){
            m_ParabolicSAR.Call();
            if (m_Oposition.Value == -1){
                m_ParLE.Send(m_oparop.Value);
            }
        }
    }
}