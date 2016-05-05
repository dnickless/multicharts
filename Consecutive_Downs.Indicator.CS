using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Consecutive_Downs : IndicatorObject
    {
        private ISeries<Double> m_price;

        private VariableObject<int> m_value1;

        private IPlotObject Plot1;

        public Consecutive_Downs(object ctx) :
            base(ctx){
            consecutivebarsdown = 3;
        }

        [Input]
        public double consecutivebarsdown { get; set; }

        protected override void Create(){
            m_value1 = new VariableObject<int>(this);
            Plot1 = AddPlot(new PlotAttributes("ConsecDn", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, true));
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
        }

        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(m_price[0], m_price[1])){
                m_value1.Value ++;
            }
            else{
                m_value1.Value = 0;
            }
            if (PublicFunctions.DoubleGreaterEquals(m_value1.Value, consecutivebarsdown)){
                Plot1.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}