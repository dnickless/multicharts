using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MovAvg3Line_Cross_LE : SignalObject
    {
        private AverageFC m_FastAverageFC;

        private AverageFC m_MedAverageFC;

        private AverageFC m_SlowAverageFC;

        private VariableSeries<Boolean> m_Cond;

        private IOrderMarket m_MA3CrsLE;

        public MovAvg3Line_Cross_LE(object ctx) :
            base(ctx)
        {
            MedLength = 9;
            SlowLength = 18;
            FastLength = 4;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int FastLength { get; set; }

        [Input]
        public int MedLength { get; set; }

        [Input]
        public int SlowLength { get; set; }

        protected override void Create(){
            m_FastAverageFC = new AverageFC(this);
            m_MedAverageFC = new AverageFC(this);
            m_SlowAverageFC = new AverageFC(this);
            m_Cond = new VariableSeries<Boolean>(this);
            m_MA3CrsLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MA3CrsLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_FastAverageFC.price = Price;
            m_FastAverageFC.length = FastLength;
            m_MedAverageFC.price = Price;
            m_MedAverageFC.length = MedLength;
            m_SlowAverageFC.price = Price;
            m_SlowAverageFC.length = SlowLength;
        }


        protected override void CalcBar(){
            double m_FastAvg = m_FastAverageFC[0];
            double m_MedAvg = m_MedAverageFC[0];
            double m_SlowAvg = m_SlowAverageFC[0];
            m_Cond.Value = PublicFunctions.DoubleGreater(Price[0], m_FastAvg) 
                &&PublicFunctions.DoubleGreater(m_FastAvg, m_MedAvg)
                &&PublicFunctions.DoubleGreater(m_MedAvg, m_SlowAvg);
            if (Bars.CurrentBar > 1 && m_Cond.Value
                 && m_Cond[1] == false){
                m_MA3CrsLE.Send();
            }
        }
    }
}