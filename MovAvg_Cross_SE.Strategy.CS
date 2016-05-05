using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MovAvg_Cross_SE : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableObject<Int32> m_Counter;

        
        private IOrderMarket m_MACrossSE;

        public MovAvg_Cross_SE(object ctx) :
            base(ctx)
        {
            Length = 9;
            ConfirmBars = 1;
        }

        private ISeries<Double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        [Input]
        public int ConfirmBars { get; set; }

        protected override void Create(){
            m_AverageFC = new AverageFC(this);
            m_MACrossSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MACrossSE",
                                                                      EOrderAction.SellShort));
            m_Counter = new VariableObject<int>(this);
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_AverageFC.price = Price;
            m_AverageFC.length = Length;
        }


        protected override void CalcBar(){
            double m_avg = m_AverageFC[0];
            if (PublicFunctions.DoubleLess(Price[0], m_avg)){
                ++m_Counter.Value;
            }
            else{
                m_Counter.Value = 0;
            }

            if (PublicFunctions.DoubleGreater(Bars.CurrentBar, ConfirmBars)
                 && m_Counter.Value == ConfirmBars)
            {
                m_MACrossSE.Send();
            }
        }
    }
}