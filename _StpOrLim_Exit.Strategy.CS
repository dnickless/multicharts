using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class _StpOrLim_Exit : SignalObject
    {
        private IOrderPriced m_SellStop;

        private IOrderPriced m_SellLimit;

        private IOrderPriced m_CoverStop;

        private IOrderPriced m_CoverLimit;

        private IOrderPriced m_ExitOrder;

        public _StpOrLim_Exit(object ctx) :
            base(ctx){
            ExitQuantity = 1000;
            TriggerPrice = 100;
            SellOrCover = true;
            StopOrLimit = true;
        }

        [Input]
        public bool SellOrCover { get; set; }

        [Input]
        public bool StopOrLimit { get; set; }

        [Input]
        public double TriggerPrice { get; set; }

        [Input]
        public int ExitQuantity { get; set; }

        protected override void Create(){
            m_SellStop =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Sell", EOrderAction.Sell,
                                                             OrderExit.FromAll));
            m_SellLimit =
                OrderCreator.Limit(new SOrderParameters(Contracts.Default, "Sell#1", EOrderAction.Sell,
                                                              OrderExit.FromAll));
            m_CoverStop =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Cover", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
            m_CoverLimit =
                OrderCreator.Limit(new SOrderParameters(Contracts.Default, "Cover#1", EOrderAction.BuyToCover,
                                                              OrderExit.FromAll));
        }

        protected override void StartCalc()
        {
            m_ExitOrder = null;

            if (SellOrCover && StopOrLimit)
            {
               m_ExitOrder = m_SellStop;
            }
            else
            {
                if (SellOrCover && !StopOrLimit)
                {
                    m_ExitOrder = m_SellLimit;
                }
                else
                {
                    if (!SellOrCover && StopOrLimit)
                    {
                        m_ExitOrder = m_CoverStop;
                    }
                    else
                    {
                        if (!SellOrCover && !StopOrLimit)
                        {
                            m_ExitOrder = m_CoverLimit;
                        }
                    }
                }
            }
            if (m_ExitOrder == null)
                throw new ApplicationException("Invalid Parameter");
        }


        protected override void CalcBar(){
            m_ExitOrder.Send(TriggerPrice, ExitQuantity);
        }
    }
}