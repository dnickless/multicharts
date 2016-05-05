namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Inside_Bar_SE : SignalObject
    {
        private IOrderMarket m_InsBarSE;

        public Inside_Bar_SE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_InsBarSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "InsBarSE", EOrderAction.SellShort));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleGreater(Bars.Low[0], Bars.Low[1])){
                if (PublicFunctions.DoubleLess(Bars.Close[0], Bars.Open[0])){
                    m_InsBarSE.Send();
                }
            }
        }
    }
}