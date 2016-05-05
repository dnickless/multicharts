using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Avg_Close_Greater_Avg_Open : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private AverageFC m_averagefc2;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Avg_Close_Greater_Avg_Open(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_averagefc2 = new AverageFC(this);
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
            m_averagefc1.price = Bars.Close;
            m_averagefc1.length = length;
            m_averagefc2.price = Bars.Open;
            m_averagefc2.length = length;
        }


        protected override void CalcBar(){
            if (PublicFunctions.DoubleGreater(m_averagefc1[0], m_averagefc2[0])){
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