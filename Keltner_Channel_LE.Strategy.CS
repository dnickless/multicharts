using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Keltner_Channel_LE : SignalObject
    {
        private AverageFC m_AverageFC;

        private VariableSeries<Double> m_UpperBand;

        private VariableObject<Boolean> m_bSetupLE;

        private VariableObject<Double> m_CrossingHigh;

        private IOrderPriced m_KltChLE;

        public Keltner_Channel_LE(object ctx) :
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
            m_UpperBand = new VariableSeries<Double>(this);
            m_bSetupLE = new VariableObject<bool>(this);
            m_CrossingHigh = new VariableObject<double>(this);
            m_KltChLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "KltChLE", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            Price = Bars.Close;
            m_AverageFC.price = Price;
            m_AverageFC.length = Length;
            
        }


        protected override void CalcBar(){
            double m_avg = m_AverageFC[0];
            double m_shift = NumAtrs*this.AverageTrueRange(Length);
            m_UpperBand.Value = m_avg + m_shift;
            if (Bars.CurrentBar > 1 && Price.CrossesOver(m_UpperBand, ExecInfo.MaxBarsBack))
            {
                m_bSetupLE.Value = true;
                m_CrossingHigh.Value = Bars.High[0];
            }
            else{
                if (m_bSetupLE.Value
                    && (PublicFunctions.DoubleLess(Price[0], m_avg)
                        || PublicFunctions.DoubleGreaterEquals(Bars.High[0], m_CrossingHigh.Value + Bars.Point)))
                    m_bSetupLE.Value = false;
            }
            if (m_bSetupLE.Value)
                m_KltChLE.Send(m_CrossingHigh.Value + Bars.Point);
        }
    }
}