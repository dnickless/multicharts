using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_Hammer_HangingMan : IndicatorObject
    {
        private Function.C_Hammer_HangingMan m_c_hammer_hangingman1;

        private VariableObject<int> m_ohammer;

        private VariableObject<int> m_ohangingman;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_Hammer_HangingMan(object ctx) :
            base(ctx){
            factor = 2;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double factor { get; set; }

        protected override void Create(){
            m_c_hammer_hangingman1 = new Function.C_Hammer_HangingMan(this);
            m_ohammer = new VariableObject<int>(this);
            m_ohangingman = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Hammer", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("HangMan", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_hammer_hangingman1.length = length;
            m_c_hammer_hangingman1.factor = factor;
            m_c_hammer_hangingman1.ohammer = m_ohammer;
            m_c_hammer_hangingman1.ohangingman = m_ohangingman;
            m_ohammer.DefaultValue = 0;
            m_ohangingman.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_hammer_hangingman1.Call();
            if (m_ohammer.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("Hammer");
            }
            else{
                if (m_ohangingman.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("HangingMan");
                }
            }
        }
    }
}