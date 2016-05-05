using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class RSI_SE : SignalObject
    {
        private RSI m_RSI;

        private VariableSeries<Double> m_myrsi;

        private IOrderMarket m_RsiSE;

        public RSI_SE(object ctx) :
            base(ctx)
        {
            OverBought = 70;
            Length = 14;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        [Input]
        public double OverBought { get; set; }

        protected override void Create(){
            m_RSI = new RSI(this);
            m_myrsi = new VariableSeries<Double>(this);
            m_RsiSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "RsiSE", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_RSI.price = Price;
            m_RSI.length = Length;
        }


        protected override void CalcBar(){
            m_myrsi.Value = m_RSI[0];
            if (Bars.CurrentBar > 1){
                if (this.CrossesUnder(m_myrsi,OverBought)){
                    m_RsiSE.Send();
                }
            }
        }
    }
}