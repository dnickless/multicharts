using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_PierceLine_DkCloud : IndicatorObject
    {
        private Function.C_PierceLine_DkCloud m_c_pierceline_dkcloud1;


        private VariableObject<int> m_opiercingline;

        private VariableObject<int> m_odarkcloud;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_PierceLine_DkCloud(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_c_pierceline_dkcloud1 = new Function.C_PierceLine_DkCloud(this);
            m_opiercingline = new VariableObject<int>(this);
            m_odarkcloud = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("PierceLine", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("DkCloud", EPlotShapes.Point,
                                           Color.Magenta, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_pierceline_dkcloud1.length = length;
            m_c_pierceline_dkcloud1.opiercingline = m_opiercingline;
            m_c_pierceline_dkcloud1.odarkcloud = m_odarkcloud;
        }


        protected override void CalcBar(){
            m_c_pierceline_dkcloud1.Call();
            if (m_opiercingline.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("PiercingLine");
            }
            else{
                if (m_odarkcloud.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("DarkCloud");
                }
            }
        }
    }
}