using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class _Historical_Entry : SignalObject
    {
        private Boolean m_InitialPass;

        private IOrderPriced m_StopBuy;

        private IOrderPriced m_LimitBuy;

        private IOrderPriced m_StopShort;

        private IOrderPriced m_LimitShort;

        public _Historical_Entry(object ctx) :
            base(ctx)
        {
            PrevBarTime = DateTime.Now;
            IsLongOrder = true;
            MyEntryPrice = 100;
            Quantity = 1000;
        }

        [Input]
        public double MyEntryPrice { get; set; }

        [Input]
        public int Quantity { get; set; }

        [Input]
        public bool IsLongOrder { get; set; }

        [Input]
        public DateTime PrevBarTime { get; set; }

        [Input]
        public int OpenPriceEntryBar { get; set; }

        protected override void Create(){
            m_StopBuy = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Buy", EOrderAction.Buy));
            m_LimitBuy = OrderCreator.Limit(new SOrderParameters(Contracts.Default, "Buy#1", EOrderAction.Buy));
            m_StopShort = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Short", EOrderAction.SellShort));
            m_LimitShort =
                OrderCreator.Limit(new SOrderParameters(Contracts.Default, "Short#1", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            m_InitialPass = true;
        }


        protected override void CalcBar()
        {
            if (m_InitialPass
				&& Bars.Time[0].Date == PrevBarTime.Date
                && (Bars.Info.Resolution.Type.IsDayBased() || Bars.Time[0] == PrevBarTime))
            {
				if (Environment.IsAutoTradingMode) {
					ChangeMarketPosition((IsLongOrder ? 1 : -1) * Quantity, MyEntryPrice);
				}
				else
					if (IsLongOrder) {
	                    if (PublicFunctions.DoubleGreater(MyEntryPrice, OpenPriceEntryBar))
	                        m_StopBuy.Send(MyEntryPrice, Quantity);
	                    else
	                        m_LimitBuy.Send(MyEntryPrice, Quantity);
	                }
	                else {
	                    if (PublicFunctions.DoubleLess(MyEntryPrice, OpenPriceEntryBar))
	                        m_StopShort.Send(MyEntryPrice, Quantity);
	                    else
	                        m_LimitShort.Send(MyEntryPrice, Quantity);
	                }
            }
            if (m_InitialPass && Bars.LastBarOnChart)
                m_InitialPass = false;
        }
    }
}