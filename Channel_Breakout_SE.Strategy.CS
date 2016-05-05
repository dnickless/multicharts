using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Channel_Breakout_SE : SignalObject
    {
        private LowestFC m_LowestFC;

        private IOrderPriced m_ChBrkSE;

        public Channel_Breakout_SE(object ctx) :
            base(ctx)
        {
            Length = 20;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_LowestFC = new LowestFC(this);
            m_ChBrkSE =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ChBrkSE", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            Price = Bars.Low;
            m_LowestFC.pricevalue = Price;
            m_LowestFC.len = Length;
        }


        protected override void CalcBar(){
            m_ChBrkSE.Send(m_LowestFC[0]-Bars.Point);
        }
    }
}