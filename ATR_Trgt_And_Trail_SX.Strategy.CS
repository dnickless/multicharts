using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class ATR_Trgt_And_Trail_SX : SignalObject
    {
        private VariableSeries<int> m_mp;

        private VariableObject<Double> m_TargetPrice;

        private IOrderPriced m_ATTSX_Tgt;

        private IOrderPriced m_ATTSX_Trl;

        public ATR_Trgt_And_Trail_SX(object ctx) :
            base(ctx){
            NumBars = 5;
            NumAtrs = 2;
            AtrLength = 5;
        }

        [Input]
        public int AtrLength { get; set; }

        [Input]
        public int NumAtrs { get; set; }

        [Input]
        public int NumBars { get; set; }

        protected override void Create(){
            m_mp = new VariableSeries<int>(this);
            m_ATTSX_Tgt =
                OrderCreator.Limit(new SOrderParameters(Contracts.Default, "ATTSX-Tgt", EOrderAction.BuyToCover,
                                                              OrderExit.FromAll));
            m_ATTSX_Trl =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ATTSX-Trl", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
            m_TargetPrice = new VariableObject<double>(this);
        }

 
        protected override void CalcBar(){
            m_mp.Value = StrategyInfo.MarketPosition;
            if (m_mp.Value < 0){
                if (m_mp[1] >= 0)
                {
                    m_TargetPrice.Value = this.EntryPrice() - this.AverageTrueRange(AtrLength)*NumAtrs;
                }
                if (this.BarsSinceEntry() < NumBars){
                    m_ATTSX_Tgt.Send(m_TargetPrice.Value);
                }
                else{
                    m_ATTSX_Trl.Send(Bars.High[0]);
                }
            }
        }
    }
}