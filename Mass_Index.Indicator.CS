using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Mass_Index : IndicatorObject
    {
        private MassIndex m_massindex1;

        private VariableSeries<Double> m_massx;

        private VariableObject<Boolean> m_setupstatus;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Mass_Index(object ctx) :
            base(ctx){
            trigger = 26.5;
            setup = 27;
            summationlength = 25;
            smoothinglength = 9;
        }

        [Input]
        public int smoothinglength { get; set; }

        [Input]
        public int summationlength { get; set; }

        [Input]
        public double setup { get; set; }

        [Input]
        public double trigger { get; set; }

        protected override void Create(){
            m_massindex1 = new MassIndex(this);
            m_massx = new VariableSeries<Double>(this);
            m_setupstatus = new VariableObject<Boolean>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MassX", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Setup", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("Trigger", 0, Color.Lime,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_massindex1.smoothinglength = smoothinglength;
            m_massindex1.summationlength = summationlength;
        }


        protected override void CalcBar(){
            m_massx.Value = m_massindex1[0];
            Plot1.Set(0, m_massx.Value);
            Plot2.Set(0, setup);
            Plot3.Set(0, trigger);
            if (this.CrossesOver(m_massx, setup)){
                m_setupstatus.Value = true;
            }
            else{
                if (m_setupstatus.Value && this.CrossesUnder(m_massx, trigger)){
                    Alerts.Alert("Reversal alert");
                    m_setupstatus.Value = false;
                }
            }
        }
    }
}