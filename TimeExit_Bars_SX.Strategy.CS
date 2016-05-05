namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class TimeExit_Bars_SX : SignalObject
    {
        private IOrderMarket m_TimeBarsSX;

        public TimeExit_Bars_SX(object ctx) :
            base(ctx)
        {
            BarToExitOn = 5;
        }

        [Input]
        public int BarToExitOn { get; set; }

        protected override void Create(){
            m_TimeBarsSX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "TimeBarsSX",
                                                                      EOrderAction.BuyToCover, OrderExit.FromAll));
        }


        protected override void CalcBar(){
            if (this.BarsSinceEntry() == BarToExitOn){
                m_TimeBarsSX.Send();
            }
        }
    }
}