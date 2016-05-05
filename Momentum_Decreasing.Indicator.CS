using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Momentum_Decreasing : IndicatorObject
    {
        private VariableSeries<Double> m_momavg;

        private IPlotObject Plot1;
        private IPlotObject Plot2;

        public Momentum_Decreasing(object ctx) :
            base(ctx){
            momavglength = 14;
            momlength = 14;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int momlength { get; set; }

        [Input]
        public int momavglength { get; set; }

        protected override void Create(){
            m_momavg = new VariableSeries<Double>(this);
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
        }

        protected override void CalcBar(){
            m_momavg.Value = price.MomentumSeries(momlength).Average(momavglength);
            if (PublicFunctions.DoubleLess(m_momavg.Value, m_momavg[1])){
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