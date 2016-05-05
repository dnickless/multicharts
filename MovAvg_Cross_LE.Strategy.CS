using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MovAvg_Cross_LE : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableObject<Int32> m_Counter;

        private IOrderMarket m_MACrossLE;

        public MovAvg_Cross_LE(object ctx) :
            base(ctx)
        {
            Length = 9;
            ConfirmBars = 1;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        [Input]
        public int ConfirmBars { get; set; }

        protected override void Create(){
            m_AverageFC = new AverageFC(this);
            m_MACrossLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MACrossLE", EOrderAction.Buy));
            m_Counter = new VariableObject<int>(this);
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_AverageFC.price = Price;
            m_AverageFC.length = Length;
        }


        protected override void CalcBar(){
            double m_Avg = m_AverageFC[0];
            if (PublicFunctions.DoubleGreater(Price[0], m_Avg)){
                ++m_Counter.Value;
            }
            else{
                m_Counter.Value = 0;
            }
            if (PublicFunctions.DoubleGreater(Bars.CurrentBar, ConfirmBars)
                 && m_Counter.Value == ConfirmBars)
            {
                m_MACrossLE.Send();
            }
        }
    }
}