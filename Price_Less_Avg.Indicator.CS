using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Price_Less_Avg : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Price_Less_Avg(object ctx) :
            base(ctx){
            length = 14;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
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

        protected override void StartCalc(){
            price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleLess(price[0], m_averagefc1[0])){
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