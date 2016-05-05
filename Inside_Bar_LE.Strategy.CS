namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Inside_Bar_LE : SignalObject
    {
        private IOrderMarket m_InsBarLE;

        public Inside_Bar_LE(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_InsBarLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "InsBarLE", EOrderAction.Buy));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.High[0], Bars.High[1]) &&
                 PublicFunctions.DoubleGreater(Bars.Low[0], Bars.Low[1])){
                if (PublicFunctions.DoubleGreater(Bars.Close[0], Bars.Open[0])){
                    m_InsBarLE.Send();
                }
            }
        }
    }
}