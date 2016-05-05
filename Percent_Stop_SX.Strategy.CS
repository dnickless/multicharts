namespace PowerLanguage.Strategy
{
    public class Percent_Stop_SX : SignalObject
    {
        private IOrderPriced m_PctStopSX_eb;

        public Percent_Stop_SX(object ctx) :
            base(ctx)
        {
            StopLossPct = 0.05;
        }

        [Input]
        public double StopLossPct { get; set; }

        protected override void Create(){
            m_PctStopSX_eb =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PctStopSX-eb", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }


        protected override void CalcBar(){
            CurSpecOrdersMode = ESpecOrdersMode.PerContract;
            if (StrategyInfo.MarketPosition < 0){
                GenerateStopLoss(this.EntryPrice()*StopLossPct);
            }
            else{
                m_PctStopSX_eb.Send(Bars.Close[0]*(1.0 + StopLossPct));
            }
        }
    }
}