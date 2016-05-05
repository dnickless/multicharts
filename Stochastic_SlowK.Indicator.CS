using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Stochastic_SlowK : IndicatorObject
    {
        private SlowKCustom m_slowkcustom1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Stochastic_SlowK(object ctx) :
            base(ctx){
            overbcolor = Color.Blue;
            overscolor = Color.Cyan;
            overbought = 80;
            oversold = 20;
            length = 14;
        }

        private ISeries<double> priceh { get; set; }

        private ISeries<double> pricel { get; set; }

        private ISeries<double> pricec { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        [Input]
        public Color overscolor { get; set; }

        [Input]
        public Color overbcolor { get; set; }

        protected override void Create(){
            m_slowkcustom1 = new SlowKCustom(this);
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
            priceh = Bars.High;
            pricel = Bars.Low;
            pricec = Bars.Close;
            m_slowkcustom1.priceh = priceh;
            m_slowkcustom1.pricel = pricel;
            m_slowkcustom1.pricec = pricec;
            m_slowkcustom1.stochlength = length;
        }


        protected override void CalcBar(){
            var m_myslowk = m_slowkcustom1[0];
            if (PublicFunctions.DoubleLess(m_myslowk, oversold)){
                Plot1.Set(0, Bars.High[0], overscolor);
                Plot2.Set(0, Bars.Low[0], overscolor);
                Alerts.Alert("SlowK in oversold zone");
            }
            else{
                if (PublicFunctions.DoubleGreater(m_myslowk, overbought)){
                    Plot1.Set(0, Bars.High[0], overbcolor);
                    Plot2.Set(0, Bars.Low[0], overbcolor);
                    Alerts.Alert("SlowK in overbought zone");
                }
                else{
                    Plot1.Reset();
                }
            }
        }
    }
}