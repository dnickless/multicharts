using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Pivot_High : IndicatorObject
    {
        private PivotHighVSBar m_pivothighvsbar1;

        private IPlotObject Plot1;

        public Pivot_High(object ctx) :
            base(ctx){
            rightstrength = 3;
            leftstrength = 3;
        }

        [Input]
        public int leftstrength { get; set; }

        [Input]
        public int rightstrength { get; set; }

        protected override void Create(){
            m_pivothighvsbar1 = new PivotHighVSBar(this);
            Plot1 =
                AddPlot(new PlotAttributes("PivotHi", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_pivothighvsbar1.instance = 1;
            m_pivothighvsbar1.price = Bars.High;
            m_pivothighvsbar1.leftstrength = leftstrength;
            m_pivothighvsbar1.rightstrength = rightstrength;
            m_pivothighvsbar1.length = (rightstrength + 1);
        }


        protected override void CalcBar(){
            if ((m_pivothighvsbar1[0]
                 != -1)){
                Plot1.Set(rightstrength, Bars.High[rightstrength]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}