using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_Low_for_Year : IndicatorObject
    {
        private VariableObject<Double> m_lowestlo;

        private IPlotObject Plot1;

        public New_Low_for_Year(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_lowestlo = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("NewLo-Y", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void CalcBar(){
            EResolution resolution = Bars.Info.Resolution.Type;
            if (resolution > EResolution.Quarter) return;

            if ((Bars.Time[0].Year != Bars.Time[1].Year)){
                m_lowestlo.Value = Bars.Low[0];
            }
            else{
                if (PublicFunctions.DoubleLess(Bars.Low[0], m_lowestlo.Value)){
                    Plot1.Set(0, Bars.Low[0]);
                    Alerts.Alert();
                    m_lowestlo.Value = Bars.Low[0];
                }
            }
        }
    }
}