using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Price_Channel_LE : SignalObject
    {
        private HighestFC m_HighestFC;

        private IOrderPriced m_PChLE;

        public Price_Channel_LE(object ctx) :
            base(ctx)
        {
            Length = 20;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_HighestFC = new HighestFC(this);
            m_PChLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PChLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            m_HighestFC.pricevalue = Bars.High;
            m_HighestFC.len = Length;
        }


        protected override void CalcBar(){
            m_PChLE.Send(m_HighestFC[0]+ Bars.Point);
        }
    }
}