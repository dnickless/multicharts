using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Price_Channel_SE : SignalObject
    {
        private LowestFC m_LowestFC;

        private IOrderPriced m_PChSE;

        public Price_Channel_SE(object ctx) :
            base(ctx)
        {
            Length = 20;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_LowestFC = new LowestFC(this);
            m_PChSE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PChSE", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            m_LowestFC.pricevalue = Bars.Low;
            m_LowestFC.len = Length;
        }


        protected override void CalcBar(){
            m_PChSE.Send(m_LowestFC[0] - Bars.Point);
        }
    }
}