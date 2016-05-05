using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MovAvg_Cross_LX : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableSeries<Double> m_Avg;

        private IOrderMarket m_MACrossLX;

        public MovAvg_Cross_LX(object ctx) :
            base(ctx)
        {
            Length = 9;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_AverageFC = new AverageFC(this);
            m_Avg = new VariableSeries<Double>(this);
            m_MACrossLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MACrossLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_AverageFC.price = Bars.Close;
            m_AverageFC.length = Length;
        }


        protected override void CalcBar(){
            m_Avg.Value = m_AverageFC[0];
            if (Bars.CurrentBar > 1 && Bars.Close.CrossesUnder(m_Avg, ExecInfo.MaxBarsBack))
            {
                m_MACrossLX.Send();
            }
        }
    }
}