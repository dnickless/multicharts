using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class PercentR_SE : SignalObject
    {
        private VariableSeries<Double> m_mypercentr;

        private VariableSeries<Double> m_avg;

        private VariableObject<Boolean> m_setups;

        private IOrderMarket m_PctRSE;

        public PercentR_SE(object ctx) :
            base(ctx)
        {
            TriggerShort = 38;
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
        public double TriggerShort { get; set; }

        protected override void Create(){
            m_mypercentr = new VariableSeries<Double>(this);
            m_avg = new VariableSeries<Double>(this);
            m_PctRSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "PctRSE",
                                                                      EOrderAction.SellShort));
            m_setups = new VariableObject<bool>(this);
        }

        protected override void CalcBar(){
            m_mypercentr.Value = Bars.PercentR(Length);
            m_avg.Value = Bars.Close.Average(Length);
            if (PublicFunctions.DoubleLess(m_mypercentr.Value, OverSold)){
                m_setups.Value = false;
            }
            else{
                if (PublicFunctions.DoubleGreater(m_mypercentr.Value, OverBought)){
                    m_setups.Value = true;
                }
            }
            if (m_setups.Value 
                && PublicFunctions.DoubleLess(m_avg.Value, m_avg[1])
                 && this.CrossesUnder(m_mypercentr,TriggerShort))
            {
                m_PctRSE.Send();
                m_setups.Value = false;
            }
        }
    }
}