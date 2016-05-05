using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class EntryBar_ATR_Stop_LX : SignalObject
    {
        private IOrderPriced m_EbAtrLX;

        public EntryBar_ATR_Stop_LX(object ctx) :
            base(ctx){
            NumAtrs = 1;
            AtrLength = 5;
        }

        [Input]
        public int AtrLength { get; set; }

        [Input]
        public int NumAtrs { get; set; }

        protected override void Create(){
            m_EbAtrLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "EbAtrLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }

        protected override void CalcBar(){
            if (StrategyInfo.MarketPosition != 1)
            {
                m_EbAtrLX.Send(Bars.Close[0] - this.AverageTrueRange(AtrLength)*NumAtrs);
            }
        }
    }
}