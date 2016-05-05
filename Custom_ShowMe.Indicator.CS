using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Custom_ShowMe : IndicatorObject
    {
        private IPlotObject Plot1;

        public Custom_ShowMe(object ctx) :
            base(ctx) {}

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("CustomSM", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
        }

        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(Bars.High[0], Bars.High[1]) &&
                            PublicFunctions.DoubleGreater(Bars.Low[0], Bars.Low[1]))
            {
                Plot1.Set(0, Bars.CloseValue);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}