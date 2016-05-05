using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Profitable_Closes_LX : SignalObject
    {
        private VariableSeries<int> m_MP;

        private VariableObject<Double> m_EntryPrice;

        private VariableObject<Int32> m_cnt;

        private IOrderMarket m_PftClsLX;

        public Profitable_Closes_LX(object ctx) :
            base(ctx)
        {
            NumProfitCloses = 5;
        }

        [Input]
        public int NumProfitCloses { get; set; }

        protected override void Create(){
            m_MP = new VariableSeries<int>(this);
            m_PftClsLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "PftClsLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
            m_EntryPrice = new VariableObject<double>(this);
            m_cnt = new VariableObject<int>(this);
        }

       protected override void CalcBar(){
            m_MP.Value = 0;
            if (StrategyInfo.MarketPosition > 0){
                m_MP.Value = 1;
            }
            if (StrategyInfo.MarketPosition < 0){
                m_MP.Value = -1;
            }
            if (m_MP.Value != 0){
                if (m_MP[1] != m_MP.Value){
                    m_cnt.Value = 0;
                    m_EntryPrice.Value = this.EntryPrice();
                }
                if (PublicFunctions.DoubleGreater(Bars.Close[0], m_EntryPrice.Value) &&
                      PublicFunctions.DoubleGreater(m_MP.Value, 0))
                {
                    ++m_cnt.Value;
                }
                if (m_cnt.Value == NumProfitCloses)
                {
                    m_PftClsLX.Send();
                }
            }
        }
    }
}