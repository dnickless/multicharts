namespace PowerLanguage.Strategy
{
    public class Percent_Stop_LX : SignalObject
    {
        private IOrderPriced m_PctStopLX_eb;

        public Percent_Stop_LX(object ctx) :
            base(ctx)
        {
            StopLossPct = 0.05;
        }

        [Input]
        public double StopLossPct { get; set; }

        protected override void Create(){
            m_PctStopLX_eb =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "PctStopLX-eb", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }


        protected override void CalcBar(){
            CurSpecOrdersMode = ESpecOrdersMode.PerContract;
            if (StrategyInfo.MarketPosition > 0){
                GenerateStopLoss(this.EntryPrice()*StopLossPct);
            }
            else{
                m_PctStopLX_eb.Send(Bars.Close[0]*(1.0 - StopLossPct));
            }
        }
    }
}