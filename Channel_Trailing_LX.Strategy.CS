using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Channel_Trailing_LX : SignalObject
    {
        private LowestFC m_LowestFC;

        private IOrderPriced m_ChTrLX;

        public Channel_Trailing_LX(object ctx) :
            base(ctx)
        {
            FloorAmt = 1;
            Length = 3;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double FloorAmt { get; set; }

        [Input]
        public bool IsPositionBasis { get; set; }

        protected override void Create(){
            m_LowestFC = new LowestFC(this);
            m_ChTrLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ChTrLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_LowestFC.pricevalue = Bars.Low;
            m_LowestFC.len = Length;
        }


        protected override void CalcBar(){
            double FloorProfit = IsPositionBasis ? FloorAmt : FloorAmt*StrategyInfo.MarketPosition;
            double SellPrice = m_LowestFC[0];
            if (StrategyInfo.MarketPosition > 0
                 && PublicFunctions.DoubleGreaterEquals(this.MaxPositionProfit(), FloorProfit))
            {
                m_ChTrLX.Send(SellPrice);
            }
        }
    }
}