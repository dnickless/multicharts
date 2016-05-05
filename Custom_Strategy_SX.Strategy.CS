using System;

namespace PowerLanguage.Strategy
{
    public class Custom_Strategy_SX : SignalObject
    {
        private IOrderMarket m_CustomSX;

        public Custom_Strategy_SX(object ctx) :
            base(ctx) {}

        private ISeries<bool> SX_Condition { get; set; }

        protected override void Create(){
            m_CustomSX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "CustomSX",
                                                                      EOrderAction.BuyToCover, OrderExit.FromAll));
        }

        protected override void StartCalc(){
            SX_Condition =
                new Lambda<Boolean>(
                    _bb => PublicFunctions.DoubleGreater(Bars.Close[_bb], Bars.Open[_bb]));
        }


        protected override void CalcBar(){
            if (SX_Condition[0]){
                m_CustomSX.Send();
            }
        }
    }
}