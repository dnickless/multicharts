using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Consecutive_Ups : IndicatorObject
    {
        private ISeries<Double> m_price;

        private VariableObject<int> m_value1;

        private IPlotObject Plot1;

        public Consecutive_Ups(object ctx) :
            base(ctx){
            consecutivebarsup = 3;
        }

        [Input]
        public double consecutivebarsup { get; set; }

        protected override void Create(){
            m_value1 = new VariableObject<int>(this);
            Plot1 = AddPlot(new PlotAttributes("ConsecUp", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, true));
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
        }
        
        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(m_price[0], m_price[1])){
                m_value1.Value ++;
            }
            else{
                m_value1.Value = 0;
            }
            if (PublicFunctions.DoubleGreaterEquals(m_value1.Value, consecutivebarsup)){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}