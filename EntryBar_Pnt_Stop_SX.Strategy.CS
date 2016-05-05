namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class EntryBar_Pnt_Stop_SX : SignalObject
    {
        private IOrderPriced m_EbPntSX;

        public EntryBar_Pnt_Stop_SX(object ctx) :
            base(ctx)
        {
            RiskPoints = 10;
        }

        [Input]
        public double RiskPoints { get; set; }

        protected override void Create(){
            m_EbPntSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "EbPntSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }


        protected override void CalcBar(){
            if (StrategyInfo.MarketPosition != -1){
                m_EbPntSX.Send(Bars.Close[0] + RiskPoints*Bars.Point);
            }
        }
    }
}