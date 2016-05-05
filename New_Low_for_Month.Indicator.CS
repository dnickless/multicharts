using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_Low_for_Month : IndicatorObject
    {
        private VariableObject<Double> m_lowestlo;

        private IPlotObject Plot1;

        public New_Low_for_Month(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_lowestlo = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("NewLo-M", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;
            if (resolution == EResolution.Year ||
                resolution == EResolution.Quarter || resolution == EResolution.Month) return;

            if ((Bars.Time[0].Month != Bars.Time[1].Month)) {
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