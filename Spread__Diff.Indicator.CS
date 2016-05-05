using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Spread__Diff : IndicatorObject
    {
        private IPlotObject Plot1;

        public Spread__Diff(object ctx) :
            base(ctx) {}

        private ISeries<double> dataseries1 { get; set; }

        private ISeries<double> dataseries2 { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("SprdDiff", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            dataseries1 = Bars.Close;
            dataseries2 = BarsOfData(2).Close;
        }


        protected override void CalcBar(){
            Plot1.Set(0, (dataseries1[0] - dataseries2[0]));
        }
    }
}