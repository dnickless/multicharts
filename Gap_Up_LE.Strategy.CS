namespace PowerLanguage.Strategy
{
    public class Gap_Up_LE : SignalObject
    {
        private IOrderMarket m_GapUp;

        public Gap_Up_LE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_GapUp =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "GapUp", EOrderAction.Buy));
        }

        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.Low[0], Bars.High[1])){
                m_GapUp.Send();
            }
        }
    }
}