using System;

namespace PowerLanguage.Strategy
{
    public class Custom_Strategy_LE : SignalObject
    {
        private IOrderMarket m_CustomLE;

        public Custom_Strategy_LE(object ctx) :
            base(ctx) {}

        private ISeries<bool> LE_Condition { get; set; }

        protected override void Create(){
            m_CustomLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "CustomLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            LE_Condition =
                new Lambda<Boolean>(
                    _bb => PublicFunctions.DoubleGreater(Bars.Close[_bb], Bars.Open[_bb]));
        }


        protected override void CalcBar(){
            if (LE_Condition[0]){
                m_CustomLE.Send();
            }
        }
    }
}