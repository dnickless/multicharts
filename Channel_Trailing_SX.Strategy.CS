using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Channel_Trailing_SX : SignalObject
    {
        private HighestFC m_HighestFC;

        private IOrderPriced m_ChTrSX;

        public Channel_Trailing_SX(object ctx) :
            base(ctx)
        {
            Length = 3;
            FloorAmt = 1;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double FloorAmt { get; set; }

        [Input]
        public bool IsPositionBasis { get; set; }

        protected override void Create(){
            m_HighestFC = new HighestFC(this);
            m_ChTrSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ChTrSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_HighestFC.pricevalue = Bars.High;
            m_HighestFC.len = Length;
        }


        protected override void CalcBar(){
            double FloorProfit = IsPositionBasis ? FloorAmt : -StrategyInfo.MarketPosition * FloorAmt; 
            double CoverPrice = m_HighestFC[0];

            if (StrategyInfo.MarketPosition < 0
                 && PublicFunctions.DoubleGreaterEquals(this.MaxPositionProfit(), FloorProfit))
            {
                m_ChTrSX.Send(CoverPrice);
            }
        }
    }
}