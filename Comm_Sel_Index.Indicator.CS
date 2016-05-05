using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Comm_Sel_Index : IndicatorObject
    {
        private CSI m_csi1;

        private IPlotObject Plot1;

        public Comm_Sel_Index(object ctx) :
            base(ctx){
            length = 14;
            mycommission = 25;
            mymargin = 1000;
        }

        [Input]
        public double mymargin { get; set; }

        [Input]
        public double mycommission { get; set; }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_csi1 = new CSI(this);
            Plot1 = AddPlot(new PlotAttributes("CSI", 0, Color.Blue, Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_csi1.mymargin = mymargin;
            m_csi1.mycommission = mycommission;
            m_csi1.length = length;
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_csi1.Value);
        }
    }
}