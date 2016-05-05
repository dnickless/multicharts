namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Key_Reversal_SE : SignalObject
    {
        private IOrderMarket m_KeyRevSE;

        public Key_Reversal_SE(object ctx) :
            base(ctx)
        {
            Length = 1;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_KeyRevSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "KeyRevSE",
                                                                      EOrderAction.SellShort));
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(Bars.High[0], Bars.High.Highest(Length, 1)) 
                &&PublicFunctions.DoubleLess(Bars.Close[0], Bars.Close[1]))
             m_KeyRevSE.Send();
        }
    }
}