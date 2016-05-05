using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class Stochastic_Slow_SE : SignalObject
    {
        private Stochastic m_Stochastic;

        private VariableSeries<Double> m_oFastK;

        private VariableSeries<Double> m_oFastD;

        private VariableSeries<Double> m_oSlowK;

        private VariableSeries<Double> m_oSlowD;

        private IOrderMarket m_StochSE;

        public Stochastic_Slow_SE(object ctx) :
            base(ctx)
        {
            Length = 14;
            OverBought = 80;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double OverBought { get; set; }

        protected override void Create(){
            m_Stochastic = new Stochastic(this);
            m_oFastK = new VariableSeries<Double>(this);
            m_oFastD = new VariableSeries<Double>(this);
            m_oSlowK = new VariableSeries<Double>(this);
            m_oSlowD = new VariableSeries<Double>(this);
            m_StochSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "StochSE",
                                                                      EOrderAction.SellShort));
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
                && m_oSlowK.CrossesUnder(m_oSlowD, ExecInfo.MaxBarsBack)
                &&PublicFunctions.DoubleGreater(m_oSlowK.Value, OverBought)){
                m_StochSE.Send();
            }
        }
    }
}