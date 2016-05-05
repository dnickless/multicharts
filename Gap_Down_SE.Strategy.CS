namespace PowerLanguage.Strategy
{
    public class Gap_Down_SE : SignalObject
    {
        private IOrderMarket m_GapDn;

        public Gap_Down_SE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_GapDn =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "GapDn", EOrderAction.SellShort));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.High[0], Bars.Low[1])){
                m_GapDn.Send();
            }
        }
    }
}