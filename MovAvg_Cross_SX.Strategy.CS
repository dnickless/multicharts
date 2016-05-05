using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MovAvg_Cross_SX : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableSeries<Double> m_Avg;

        private IOrderMarket m_MACrossSX;

        public MovAvg_Cross_SX(object ctx) :
            base(ctx)
        {
            Length = 9;
        }

        [Input]
        public int Length { get; set; }

        protected override void Create(){
            m_AverageFC = new AverageFC(this);
            m_Avg = new VariableSeries<Double>(this);
            m_MACrossSX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MACrossSX",
                                                                      EOrderAction.BuyToCover, OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_AverageFC.price = Bars.Close;
            m_AverageFC.length = Length;
        }


        protected override void CalcBar(){
            m_Avg.Value = m_AverageFC[0];

            if (Bars.CurrentBar > 1 && Bars.Close.CrossesOver(m_Avg, ExecInfo.MaxBarsBack))
            {
                m_MACrossSX.Send();
            }
        }
    }
}