using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class RSI_LE : SignalObject
    {
        private RSI m_RSI;

        private VariableSeries<Double> m_myrsi;

        private IOrderMarket m_RsiLE;

        public RSI_LE(object ctx) :
            base(ctx)
        {
            OverSold = 30;
            Length = 14;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        [Input]
        public double OverSold { get; set; }

        protected override void Create(){
            m_RSI = new RSI(this);
            m_myrsi = new VariableSeries<Double>(this);
            m_RsiLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "RsiLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_RSI.price = Price;
            m_RSI.length = Length;
        }


        protected override void CalcBar(){
            m_myrsi.Value = m_RSI[0];
            if (Bars.CurrentBar > 1){
                if (this.CrossesOver(m_myrsi,OverSold)){
                    m_RsiLE.Send();
                }
            }
        }
    }
}