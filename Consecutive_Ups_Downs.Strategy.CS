using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Consecutive_Ups_Downs : SignalObject
    {
        private ISeries<Double> m_price;

        private VariableObject<int> m_ups_cnt;

        private VariableObject<int> m_downs_cnt;

        private IOrderMarket m_ConsUpLE;

        private IOrderMarket m_ConsDnSE;

        public Consecutive_Ups_Downs(object ctx) :
            base(ctx)
        {
            ConsecutiveBarsUp = 3;
            ConsecutiveBarsDown = 3;
        }

        private ISeries<Double> price{
            get { return m_price; }
        }

        [Input]
        public int ConsecutiveBarsUp { get; set; }

        [Input]
        public int ConsecutiveBarsDown { get; set; }

        protected override void Create(){
            m_ConsUpLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "ConsUpLE", EOrderAction.Buy));
            m_ConsDnSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "ConsDnSE",
                                                                      EOrderAction.SellShort));
            m_ups_cnt = new VariableObject<int>(this);
            m_downs_cnt = new VariableObject<int>(this);
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(price[0], price[1])){
                ++m_ups_cnt.Value;
            }
            else{
                m_ups_cnt.Value = 0;
            }
            if (PublicFunctions.DoubleLess(price[0], price[1])){
               ++m_downs_cnt.Value;
            }
            else{
                m_downs_cnt.Value = 0;
            }
            if (m_ups_cnt.Value >= ConsecutiveBarsUp){
                m_ConsUpLE.Send();
            }
            if (m_downs_cnt.Value >= ConsecutiveBarsDown){
                m_ConsDnSE.Send();
            }
        }
    }
}