using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class ATR_Trailing_SX : SignalObject
    {
     
        private VariableSeries<int> m_mp;

        private VariableObject<Double> m_PosLow;

        private IOrderPriced m_AtrSX;

        private IOrderPriced m_AtrSX_eb;

        public ATR_Trailing_SX(object ctx) :
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
            m_PosLow = new VariableObject<Double>(this);
            m_AtrSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "AtrSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
            m_AtrSX_eb =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "AtrSX-eb", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }


        protected override void CalcBar(){
            double AtrVal = this.AverageTrueRange(AtrLength) * NumAtrs;
            m_mp.Value = StrategyInfo.MarketPosition;
            if (m_mp.Value < 0)
            {
                if (m_mp[1] >= 0 || PublicFunctions.DoubleLess(Bars.Low[0], m_PosLow.Value))
                {
                    m_PosLow.Value = Bars.Low[0];
                }
                m_AtrSX.Send(m_PosLow.Value + AtrVal);
            }
            else{
                m_AtrSX_eb.Send(Bars.Low[0] + AtrVal);
            }
        }
    }
}