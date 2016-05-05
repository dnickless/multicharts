using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class ATR_Trgt_And_Trail_LX : SignalObject
    {
        private VariableSeries<int> m_mp;

        private VariableObject<Double> m_TargetPrice;

        private IOrderPriced m_ATTLX_Tgt;

        private IOrderPriced m_ATTLX_Trl;

        public ATR_Trgt_And_Trail_LX(object ctx) :
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
            m_ATTLX_Tgt =
                OrderCreator.Limit(new SOrderParameters(Contracts.Default, "ATTLX-Tgt", EOrderAction.Sell,
                                                              OrderExit.FromAll));
            m_ATTLX_Trl =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ATTLX-Trl", EOrderAction.Sell,
                                                             OrderExit.FromAll));
            m_TargetPrice = new VariableObject<double>(this);
        }

      
        protected override void CalcBar(){
            m_mp.Value = StrategyInfo.MarketPosition;
            if (m_mp.Value > 0){
                if (m_mp[1] <= 0){
                    m_TargetPrice.Value = this.EntryPrice() + this.AverageTrueRange(AtrLength) *NumAtrs;
                }
                if (this.BarsSinceEntry() < NumBars){
                    m_ATTLX_Tgt.Send(m_TargetPrice.Value);
                }
                else{
                    m_ATTLX_Trl.Send(Bars.Low[0]);
                }
            }
        }
    }
}