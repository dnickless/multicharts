using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Spread__Ratio : IndicatorObject
    {
        private IPlotObject Plot1;

        public Spread__Ratio(object ctx) :
            base(ctx) {}

        private ISeries<double> dataseries1 { get; set; }

        private ISeries<double> dataseries2 { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("SprdRatio", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            dataseries1 = Bars.Close;
            dataseries2 = BarsOfData(2).Close;
        }


        protected override void CalcBar(){
            if ((dataseries2[0] != 0)){
                Plot1.Set(0, (dataseries1[0]/((dataseries2[0]))));
            }
        }
    }
}