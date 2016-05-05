using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Pivot_Extension_LE : SignalObject
    {
        private PivotLowVS m_PivotLowVS;

        private IOrderMarket m_PivExtLE;

        public Pivot_Extension_LE(object ctx) :
            base(ctx)
        {
            RightStrength = 2;
            LeftStrength = 4;
        }

        private ISeries<double> LPrice { get; set; }

        [Input]
        public int LeftStrength { get; set; }

        [Input]
        public int RightStrength { get; set; }

        protected override void Create(){
            m_PivotLowVS = new PivotLowVS(this);
            m_PivExtLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "PivExtLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            LPrice = Bars.Low;
            m_PivotLowVS.instance = 1;
            m_PivotLowVS.price = LPrice;
            m_PivotLowVS.leftstrength = LeftStrength;
            m_PivotLowVS.rightstrength = RightStrength;
            m_PivotLowVS.length = RightStrength + 1;
        }


        protected override void CalcBar(){
            if (m_PivotLowVS[0] > 0){
                m_PivExtLE.Send();
            }
        }
    }
}