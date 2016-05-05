using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Percent_Increase : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Percent_Increase(object ctx) :
            base(ctx){
            pctup = 2;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double pctup { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.Yellow, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreaterEquals(Bars.Close.RateOfChange(length), pctup))
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