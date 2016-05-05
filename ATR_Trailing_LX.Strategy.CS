using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class ATR_Trailing_LX : SignalObject
    {
        private VariableSeries<int> m_mp;

        private VariableObject<Double> m_PosHigh;

        private IOrderPriced m_AtrLX;

        private IOrderPriced m_AtrLX_eb;

        public ATR_Trailing_LX(object ctx) :
            base(ctx){
            NumAtrs = 3;
            AtrLength = 10;
        }

        [Input]
        public int AtrLength { get; set; }

        [Input]
        public int NumAtrs { get; set; }

        protected override void Create(){
            m_mp = new VariableSeries<int>(this);
            m_PosHigh = new VariableObject<Double>(this);
            m_AtrLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "AtrLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
            m_AtrLX_eb =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "AtrLX-eb", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }

        protected override void CalcBar(){
            var AtrValue = this.AverageTrueRange(AtrLength) *NumAtrs;
            m_mp.Value = StrategyInfo.MarketPosition;
            
            if (m_mp.Value > 0){
                if (m_mp[1] <= 0 || PublicFunctions.DoubleGreater(Bars.High[0], m_PosHigh.Value))
                    m_PosHigh.Value = Bars.High[0];
                m_AtrLX.Send(m_PosHigh.Value - AtrValue);
            }
            else
                m_AtrLX_eb.Send(Bars.High[0] - AtrValue);
        }
    }
}