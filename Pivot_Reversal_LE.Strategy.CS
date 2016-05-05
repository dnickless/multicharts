using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Pivot_Reversal_LE : SignalObject
    {
        private SwingHigh m_SwingHigh;

        
        private VariableObject<Boolean> m_Setup;

        private VariableObject<Double> m_PivotHigh;

        private IOrderPriced m_PivRevLE;

        public Pivot_Reversal_LE(object ctx) :
            base(ctx)
        {
            Strength = 4;
        }

        [Input]
        public int Strength { get; set; }

        protected override void Create(){
            m_SwingHigh = new SwingHigh(this);
            m_PivRevLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PivRevLE", EOrderAction.Buy));
            m_Setup = new VariableObject<bool>(this);
            m_PivotHigh = new VariableObject<double>(this);
        }

        protected override void StartCalc(){
            m_SwingHigh.instance = 1;
            m_SwingHigh.price = Bars.High;
            m_SwingHigh.strength = Strength;
            m_SwingHigh.length = Strength + 1;           
        }


        protected override void CalcBar(){
            if (m_SwingHigh[0] > 0){
                m_Setup.Value = true;
                m_PivotHigh.Value = Bars.High[Strength];
            }
            else{
                if (m_Setup.Value && PublicFunctions.DoubleGreaterEquals(Bars.High[0], m_PivotHigh.Value + Bars.Point))
                    m_Setup.Value = false;
            }
            if (m_Setup.Value)
                m_PivRevLE.Send(m_PivotHigh.Value + Bars.Point);
        }
    }
}