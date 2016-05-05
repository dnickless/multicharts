using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_Low_for_Chart : IndicatorObject
    {
        private Double m_lowestlo;

        private IPlotObject Plot1;

        public New_Low_for_Chart(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("NewLo-Ch", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_lowestlo = Bars.Low[0];
        }

        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.Low[0], m_lowestlo)){
                Plot1.Set(0, Bars.Low[0]);
                Alerts.Alert();
                m_lowestlo = Bars.Low[0];
            }
        }
    }
}