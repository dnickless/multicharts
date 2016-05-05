using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_High_for_Day : IndicatorObject
    {
        private VariableObject<Double> m_highesthi;

        private IPlotObject Plot1;

        public New_High_for_Day(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_highesthi = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("NewHi-D", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;

            if ( resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter )
            {
                if ( Bars.Time[0].Date != Bars.Time[1].Date )
                {
                    m_highesthi.Value = Bars.High[0];
                }
                else{
                    if (PublicFunctions.DoubleGreater(Bars.High[0], m_highesthi.Value))
                    {
                        Plot1.Set(0, Bars.High[0]);
                        Alerts.Alert();
                        m_highesthi.Value = Bars.High[0];
                    }
                }
            }
        }
    }
}