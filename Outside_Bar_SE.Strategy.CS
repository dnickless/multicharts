namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Outside_Bar_SE : SignalObject
    {
        private IOrderMarket m_OutBarSE;

        public Outside_Bar_SE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_OutBarSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "OutBarSE",
                                                                      EOrderAction.SellShort));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low[1])){
                if (PublicFunctions.DoubleLess(Bars.Close[0], Bars.Open[0])){
                    m_OutBarSE.Send();
                }
            }
        }
    }
}