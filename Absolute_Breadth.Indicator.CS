using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Absolute_Breadth : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Absolute_Breadth(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("AbsBreadth", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            Plot1.Set(Math.Abs(Bars.Close.Value - BarsOfData(2).Close.Value));
            Plot2.Set(0);
        }
    }
}