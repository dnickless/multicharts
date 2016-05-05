using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Custom_PaintBar : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Custom_PaintBar(object ctx) :
            base(ctx) {}

        private ISeries<bool> criteria { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Magenta, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Magenta, Color.Empty, 1,
                                           1,
                                           true));
        }

        protected override void StartCalc(){
            criteria =
                new Lambda<Boolean>(
                    _bb => (PublicFunctions.DoubleLess(Bars.High[_bb], Bars.High[(1 + _bb)]) &&
                            PublicFunctions.DoubleGreater(Bars.Low[_bb], Bars.Low[(1 + _bb)])));
        }


        protected override void CalcBar(){
            if (criteria[0]){
                Plot1.Set(0, Bars.High[0]);
                Plot2.Set(0, Bars.Low[0]);
                Alerts.Alert();
            }
            else{
                Plot1.Reset();
            }
        }
    }
}