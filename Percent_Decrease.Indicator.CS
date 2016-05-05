using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Percent_Decrease : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Percent_Decrease(object ctx) :
            base(ctx){
            pctdn = -2;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double pctdn { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Red, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Red, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void CalcBar(){
            if (PublicFunctions.DoubleLessEquals(Bars.Close.RateOfChange(length), pctdn))
            {
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