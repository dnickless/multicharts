using System;

namespace PowerLanguage.Strategy
{
    public class Custom_Strategy_LX : SignalObject
    {
        private IOrderMarket m_CustomLX;

        public Custom_Strategy_LX(object ctx) :
            base(ctx) {}

        private ISeries<bool> LX_Condition { get; set; }

        protected override void Create(){
            m_CustomLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "CustomLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
        }

        protected override void StartCalc(){
            LX_Condition =
                new Lambda<Boolean>(
                    _bb => PublicFunctions.DoubleLess(Bars.Close[_bb], Bars.Open[_bb]));
        }


        protected override void CalcBar(){
            if (LX_Condition[0]){
                m_CustomLX.Send();
            }
        }
    }
}