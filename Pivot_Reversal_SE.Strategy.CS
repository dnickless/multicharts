using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Pivot_Reversal_SE : SignalObject
    {
        private SwingLow m_SwingLow;

        private VariableObject<Boolean> m_Setup;

        private VariableObject<Double> m_PivotLow;

        private IOrderPriced m_PivRevSE;

        public Pivot_Reversal_SE(object ctx) :
            base(ctx)
        {
            Strength = 4;
        }

        [Input]
        public int Strength { get; set; }

        protected override void Create(){
            m_SwingLow = new SwingLow(this);
            m_Setup = new VariableObject<bool>(this);
            m_PivotLow = new VariableObject<double>(this);
            m_PivRevSE =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PivRevSE", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            m_SwingLow.instance = 1;
            m_SwingLow.price = Bars.Low;
            m_SwingLow.strength = Strength;
            m_SwingLow.length = Strength + 1;
            
        }


        protected override void CalcBar(){
            if (m_SwingLow[0] > 0){
                m_Setup.Value = true;
                m_PivotLow.Value = Bars.Low[Strength];
            }
            else{
                if (m_Setup.Value && PublicFunctions.DoubleLessEquals(Bars.Low[0], m_PivotLow.Value - Bars.Point))
                    m_Setup.Value = false;
            }
            if (m_Setup.Value)
                m_PivRevSE.Send(m_PivotLow.Value - Bars.Point);
        }
    }
}