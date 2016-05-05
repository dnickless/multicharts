using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Price_Greate_Avg : IndicatorObject
    {
        private ISeries<Double> m_price;

        private int m_length = 14;

        private AverageFC m_averagefc1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Price_Greate_Avg(object ctx) :
            base(ctx) {}

        private ISeries<Double> price{
            get { return m_price; }
        }

        [Input]
        public int length{
            get { return m_length; }
            set { m_length = value; }
        }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
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
            m_price = Bars.Close;
            m_averagefc1.price = price;
            m_averagefc1.length = length;
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(price[0], m_averagefc1[0])){
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