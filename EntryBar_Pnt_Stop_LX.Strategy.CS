namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class EntryBar_Pnt_Stop_LX : SignalObject
    {
        private IOrderPriced m_EbPntLX;

        public EntryBar_Pnt_Stop_LX(object ctx) :
            base(ctx)
        {
            RiskPoints = 10;
        }

        [Input]
        public double RiskPoints { get; set; }

        protected override void Create(){
            m_EbPntLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "EbPntLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }


        protected override void CalcBar(){
            if (StrategyInfo.MarketPosition != 1){
                m_EbPntLX.Send(Bars.Close[0] - RiskPoints*Bars.Point);
            }
        }
    }
}