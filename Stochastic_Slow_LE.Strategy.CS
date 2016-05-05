using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class Stochastic_Slow_LE : SignalObject
    {
        private Stochastic m_Stochastic;

        
        private VariableSeries<Double> m_oFastK;

        private VariableSeries<Double> m_oFastD;

        private VariableSeries<Double> m_oSlowK;

        private VariableSeries<Double> m_oSlowD;

        private IOrderMarket m_StochLE;

        public Stochastic_Slow_LE(object ctx) :
            base(ctx)
        {
            Length = 14;
            OverSold = 20;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double OverSold { get; set; }

        protected override void Create(){
            m_Stochastic = new Stochastic(this);
            m_oFastK = new VariableSeries<Double>(this);
            m_oFastD = new VariableSeries<Double>(this);
            m_oSlowK = new VariableSeries<Double>(this);
            m_oSlowD = new VariableSeries<Double>(this);
            m_StochLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "StochLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            m_Stochastic.priceh = Bars.High;
            m_Stochastic.pricel = Bars.Low;
            m_Stochastic.pricec = Bars.Close;
            m_Stochastic.stochlength = Length;
            m_Stochastic.length1 = 3;
            m_Stochastic.length2 = 3;
            m_Stochastic.smoothingtype = 1;
            m_Stochastic.ofastk = m_oFastK;
            m_Stochastic.ofastd = m_oFastD;
            m_Stochastic.oslowk = m_oSlowK;
            m_Stochastic.oslowd = m_oSlowD;
        }


        protected override void CalcBar(){
            m_Stochastic.Call();
            if (Bars.CurrentBar > 2
                && m_oSlowK.CrossesOver(m_oSlowD, ExecInfo.MaxBarsBack)
                &&PublicFunctions.DoubleLess(m_oSlowK.Value, OverSold)){
                m_StochLE.Send();
            }
        }
    }
}