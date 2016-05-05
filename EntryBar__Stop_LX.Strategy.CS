namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class EntryBar__Stop_LX : SignalObject
    {
        private double m_BigPointRisk;

        private IOrderPriced m_EbDlrLX;

        public EntryBar__Stop_LX(object ctx) :
            base(ctx)
        {
            DollarRisk = 1;
        }

        [Input]
        public double DollarRisk { get; set; }

        protected override void Create(){
            m_EbDlrLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "EbDlrLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_BigPointRisk = DollarRisk/Bars.Info.BigPointValue;
        }


        protected override void CalcBar(){
            if (StrategyInfo.MarketPosition != 1){
                m_EbDlrLX.Send(Bars.Close[0] - m_BigPointRisk);
            }
        }
    }
}