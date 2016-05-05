namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Key_Reversal_LX : SignalObject
    {
        private IOrderMarket m_KeyRevLX;

        public Key_Reversal_LX(object ctx) :
            base(ctx)
        {
            Length = 1;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_KeyRevLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "KeyRevLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.High[0], Bars.High.Highest(Length, 1))
                && PublicFunctions.DoubleLess(Bars.Close[0], Bars.Close[1]))
             m_KeyRevLX.Send();
        }
    }
}