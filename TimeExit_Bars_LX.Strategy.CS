namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class TimeExit_Bars_LX : SignalObject
    {
        private IOrderMarket m_TimeBarsLX;

        public TimeExit_Bars_LX(object ctx) :
            base(ctx)
        {
            BarToExitOn = 5;
        }

        [Input]
        public int BarToExitOn { get; set; }

        protected override void Create(){
            m_TimeBarsLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "TimeBarsLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
        }


        protected override void CalcBar(){
            if (this.BarsSinceEntry() == BarToExitOn){
                m_TimeBarsLX.Send();
            }
        }
    }
}