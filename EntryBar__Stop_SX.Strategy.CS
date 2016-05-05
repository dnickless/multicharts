namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class EntryBar__Stop_SX : SignalObject
    {
        private double m_BigPointRisk;

        private IOrderPriced m_EbDlrSX;

        public EntryBar__Stop_SX(object ctx) :
            base(ctx)
        {
            DollarRisk = 1;
        }

        [Input]
        public double DollarRisk { get; set; }

        protected override void Create(){
            m_EbDlrSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "EbDlrSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_BigPointRisk = DollarRisk/Bars.Info.BigPointValue;
        }


        protected override void CalcBar(){
            if (StrategyInfo.MarketPosition != -1){
                m_EbDlrSX.Send(Bars.Close[0] + m_BigPointRisk);
            }
        }
    }
}