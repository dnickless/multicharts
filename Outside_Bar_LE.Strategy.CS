namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Outside_Bar_LE : SignalObject
    {
        private IOrderMarket m_OutBarLE;

        public Outside_Bar_LE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_OutBarLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "OutBarLE", EOrderAction.Buy));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low[1])){
                if (PublicFunctions.DoubleGreater(Bars.Close[0], Bars.Open[0])){
                    m_OutBarLE.Send();
                }
            }
        }
    }
}