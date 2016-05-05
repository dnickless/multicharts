using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Keltner_Channel_SE : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableSeries<Double> m_LowerBand;

        private VariableObject<Boolean> m_bSetupSE;

        private VariableObject<Double> m_CrossingLow;

        private IOrderPriced m_KltChSE;

        public Keltner_Channel_SE(object ctx) :
            base(ctx){
            NumAtrs = 1.5;
            Length = 20;
        }

        private ISeries<double> Price { get; set; }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_AverageFC = new AverageFC(this);
            m_LowerBand = new VariableSeries<Double>(this);
            m_KltChSE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "KltChSE", EOrderAction.SellShort));
            m_bSetupSE = new VariableObject<bool>(this);
            m_CrossingLow = new VariableObject<double>(this);
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_AverageFC.price = Price;
            m_AverageFC.length = Length;
        }


        protected override void CalcBar(){
            var m_avg = m_AverageFC[0];
            var m_shift = NumAtrs*this.AverageTrueRange(Length);
            m_LowerBand.Value = m_avg - m_shift;
            if (Bars.CurrentBar > 1 && Price.CrossesUnder(m_LowerBand, ExecInfo.MaxBarsBack))
            {
                m_bSetupSE.Value = true;
                m_CrossingLow.Value = Bars.Low[0];
            }
            else{
                if (m_bSetupSE.Value
                    && (PublicFunctions.DoubleGreater(Price[0], m_avg)
                    || PublicFunctions.DoubleLessEquals(Bars.Low[0], m_CrossingLow.Value - Bars.Point)))
                {
                    m_bSetupSE.Value = false;
                }
            }
            if (m_bSetupSE.Value)
            {
                m_KltChSE.Send(m_CrossingLow.Value - Bars.Point);
            }
        }
    }
}