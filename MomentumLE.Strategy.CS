using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class MomentumLE : SignalObject
    {
        private VariableSeries<Double> m_mom;

       
        private IOrderPriced m_MomLE;

        public MomentumLE(object ctx) :
            base(ctx)
        {
            Length = 12;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_mom = new VariableSeries<Double>(this);
            m_MomLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "MomLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
        }


        protected override void CalcBar(){
            m_mom.Value = Price.Momentum(Length);
            double m_accel = m_mom.Momentum(1);
            if (PublicFunctions.DoubleGreater(m_mom.Value, 0) 
                && PublicFunctions.DoubleGreater(m_accel, 0))
            {
                m_MomLE.Send(Bars.High[0] + Bars.Point);
            }
        }
    }
}