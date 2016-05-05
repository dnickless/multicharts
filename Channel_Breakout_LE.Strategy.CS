using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Channel_Breakout_LE : SignalObject
    {
        private HighestFC m_HighestFC;

        private IOrderPriced m_ChBrkLE;

        public Channel_Breakout_LE(object ctx) :
            base(ctx)
        {
            Length = 20;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_HighestFC = new HighestFC(this);
            m_ChBrkLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ChBrkLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            Price = Bars.High;
            m_HighestFC.pricevalue = Price;
            m_HighestFC.len = Length;
        }


        protected override void CalcBar(){
            m_ChBrkLE.Send(m_HighestFC[0] + Bars.Point);
        }
    }
}