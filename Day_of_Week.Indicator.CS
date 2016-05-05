using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Day_of_Week : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Day_of_Week(object ctx) :
            base(ctx)
        {
            dayofwk = DayOfWeek.Monday;
        }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
        }

        [Input]
        public DayOfWeek dayofwk { get; set; }

        protected override void CalcBar(){
            if ((Bars.Time[0].DayOfWeek == dayofwk)){
                Plot1.Set(0, Bars.High[0]);
                Plot2.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
        }
    }
}