using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class PercentR_LE : SignalObject
    {
        private VariableSeries<Double> m_mypercentr;

        private VariableSeries<Double> m_avg;

        private VariableObject<Boolean> m_setupl;

        private IOrderMarket m_PctRLE;

        public PercentR_LE(object ctx) :
            base(ctx)
        {
            TriggerLong = 62;
            OverBought = 80;
            OverSold = 20;
            Length = 14;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double OverSold { get; set; }

        [Input]
        public double OverBought { get; set; }

        [Input]
        public double TriggerLong { get; set; }


        protected override void Create(){
            m_mypercentr = new VariableSeries<Double>(this);
            m_avg = new VariableSeries<Double>(this);
            m_PctRLE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "PctRLE", EOrderAction.Buy));
            m_setupl = new VariableObject<bool>(this);
        }

        protected override void CalcBar(){
            m_mypercentr.Value = Bars.PercentR(Length);
            m_avg.Value = Bars.Close.Average(Length);
            if (PublicFunctions.DoubleLess(m_mypercentr.Value, OverSold)){
                m_setupl.Value = true;
            }
            else{
                if (PublicFunctions.DoubleGreater(m_mypercentr.Value, OverBought)){
                    m_setupl.Value = false;
                }
            }

            if (m_setupl.Value 
                && PublicFunctions.DoubleGreater(m_avg.Value, m_avg[1])
                && this.CrossesOver(m_mypercentr, TriggerLong))
            {
                m_PctRLE.Send();
                m_setupl.Value = false;
            }
        }
    }
}