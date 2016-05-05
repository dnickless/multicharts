using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Crossover : IndicatorObject
    {
        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Mov_Avg_Crossover(object ctx) :
            base(ctx){
            dncolor = Color.Blue;
            upcolor = Color.Cyan;
            slowlength = 14;
            fastlength = 7;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
        }


        protected override void CalcBar(){
            var m_fastavg = price.Average(fastlength);
            var m_slowavg = price.Average(slowlength);
            if (PublicFunctions.DoubleGreater(m_fastavg, m_slowavg)){
                Plot1.Set(0, Bars.High[0], upcolor);
                Plot2.Set(0, Bars.Low[0], upcolor);
                Alerts.Alert("FastAvg above SlowAvg");
            }
            else{
                if (PublicFunctions.DoubleLess(m_fastavg, m_slowavg)){
                    Plot1.Set(0, Bars.High[0], dncolor);
                    Plot2.Set(0, Bars.Low[0], dncolor);
                    Alerts.Alert("FastAvg below SlowAvg");
                }
                else{
                    Plot1.Reset();
                }
            }
        }
    }
}