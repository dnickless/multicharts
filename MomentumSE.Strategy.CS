using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class MomentumSE : SignalObject
    {
        private VariableSeries<Double> m_mom;

        private IOrderPriced m_MomSE;

        public MomentumSE(object ctx) :
            base(ctx)
        {
            Length = 12;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_mom = new VariableSeries<Double>(this);
            m_MomSE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "MomSE", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
         }


        protected override void CalcBar(){
            m_mom.Value = Price.Momentum(Length);
            double m_accel = m_mom.Momentum(1);
            if (PublicFunctions.DoubleLess(m_mom.Value, 0) 
                && PublicFunctions.DoubleLess(m_accel, 0))
            {
                m_MomSE.Send(Bars.Low[0]- Bars.Point);
            }
        }
    }
}