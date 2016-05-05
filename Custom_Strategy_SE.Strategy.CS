using System;

namespace PowerLanguage.Strategy
{
    public class Custom_Strategy_SE : SignalObject
    {
        private IOrderMarket m_CustomSE;

        public Custom_Strategy_SE(object ctx) :
            base(ctx) {}

        private ISeries<bool> SE_Condition { get; set; }

        protected override void Create(){
            m_CustomSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "CustomSE",
                                                                      EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            SE_Condition =
                new Lambda<Boolean>(
                    _bb => PublicFunctions.DoubleLess(Bars.Close[_bb], Bars.Open[_bb]));
        }


        protected override void CalcBar(){
            if (SE_Condition[0]){
                m_CustomSE.Send();
            }
        }
    }
}