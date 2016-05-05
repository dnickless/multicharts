using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Pivot_Low : IndicatorObject
    {
        private PivotLowVSBar m_pivotlowvsbar1;

        private IPlotObject Plot1;

        public Pivot_Low(object ctx) :
            base(ctx){
            rightstrength = 3;
            leftstrength = 3;
        }

        [Input]
        public int leftstrength { get; set; }

        [Input]
        public int rightstrength { get; set; }

        protected override void Create(){
            m_pivotlowvsbar1 = new PivotLowVSBar(this);
            Plot1 =
                AddPlot(new PlotAttributes("PivotLo", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_pivotlowvsbar1.instance = 1;
            m_pivotlowvsbar1.price = Bars.Low;
            m_pivotlowvsbar1.leftstrength = leftstrength;
            m_pivotlowvsbar1.rightstrength = rightstrength;
            m_pivotlowvsbar1.length = (rightstrength + 1);
        }


        protected override void CalcBar(){
            if ((m_pivotlowvsbar1[0]
                 != (-1))){
                Plot1.Set(rightstrength, Bars.Low[rightstrength]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}