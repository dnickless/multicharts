namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Key_Reversal_SX : SignalObject
    {
        private IOrderMarket m_KeyRevSX;

        public Key_Reversal_SX(object ctx) :
            base(ctx)
        {
            Length = 1;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create()
        {
            m_KeyRevSX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "KeyRevSX",
                                                                EOrderAction.BuyToCover, OrderExit.FromAll));
        }


        protected override void CalcBar()
        {
            if (PublicFunctions.DoubleLess(Bars.Low[0], Bars.Low.Lowest(Length, 1)) 
                &&PublicFunctions.DoubleGreater(Bars.Close[0], Bars.Close[1]))
             m_KeyRevSX.Send();
            
        }
    }
}