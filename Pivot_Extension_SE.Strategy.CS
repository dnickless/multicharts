using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Pivot_Extension_SE : SignalObject
    {
        private PivotHighVS m_PivotHighVS;

        private IOrderMarket m_PivExtSE;

        public Pivot_Extension_SE(object ctx) :
            base(ctx)
        {
            RightStrength = 2;
            LeftStrength = 4;
        }

        private ISeries<double> HPrice { get; set; }

        [Input]
        public int LeftStrength { get; set; }

        [Input]
        public int RightStrength { get; set; }

        protected override void Create(){
            m_PivotHighVS = new PivotHighVS(this);
            m_PivExtSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "PivExtSE",
                                                                      EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            HPrice = Bars.High;
            m_PivotHighVS.instance = 1;
            m_PivotHighVS.price = HPrice;
            m_PivotHighVS.leftstrength = LeftStrength;
            m_PivotHighVS.rightstrength = RightStrength;
            m_PivotHighVS.length = RightStrength + 1;
        }


        protected override void CalcBar(){
            if (m_PivotHighVS[0] > 0){
                m_PivExtSE.Send();
            }
        }
    }
}