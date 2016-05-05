using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_3WhSolds_3BlkCrows : IndicatorObject
    {
        private Function.C_3WhSolds_3BlkCrows m_c_3whsolds_3blkcrows1;

        private VariableObject<int> m_o3whitesoldiers;

        private VariableObject<Int32> m_o3blackcrows;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_3WhSolds_3BlkCrows(object ctx) :
            base(ctx){
            percent = 5;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double percent { get; set; }

        protected override void Create(){
            m_c_3whsolds_3blkcrows1 = new Function.C_3WhSolds_3BlkCrows(this);
            m_o3whitesoldiers = new VariableObject<int>(this);
            m_o3blackcrows = new VariableObject<Int32>(this);
            Plot1 =
                AddPlot(new PlotAttributes("3WhSolds", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("3BlkCrows", EPlotShapes.Point,
                                           Color.Magenta, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_3whsolds_3blkcrows1.length = length;
            m_c_3whsolds_3blkcrows1.percent = percent;
            m_c_3whsolds_3blkcrows1.o3whitesoldiers = m_o3whitesoldiers;
            m_c_3whsolds_3blkcrows1.o3blackcrows = m_o3blackcrows;
            m_o3whitesoldiers.DefaultValue = 0;
            m_o3blackcrows.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_3whsolds_3blkcrows1.Call();
            if (m_o3whitesoldiers.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("3WhiteSoldiers");
            }
            else{
                if (m_o3blackcrows.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("3BlackCrows");
                }
            }
        }
    }
}