using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Volume : IndicatorObject
    {
        private IPlotObject Plot1;

        public Volume(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Volume", EPlotShapes.Histogram,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
        }

        private ISeries<Double> m_volume;

        protected override void StartCalc()
        {
            m_volume = Bars.TrueVolume();
        }


        protected override void CalcBar()
        {
            Plot1.Set(0, m_volume.Value);
        }
    }
}