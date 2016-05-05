using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Stochastic_Slow : IndicatorObject
    {
        private Stochastic m_stochastic1;

        private VariableSeries<Double> m_ofastk;

        private VariableSeries<Double> m_ofastd;

        private VariableSeries<Double> m_oslowk;

        private VariableSeries<Double> m_oslowd;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Stochastic_Slow(object ctx) :
            base(ctx){
            overbought = 80;
            oversold = 20;
            smoothingtype = 1;
            smoothinglength2 = 3;
            smoothinglength1 = 3;
            stochlength = 14;
        }

        private ISeries<double> priceh { get; set; }

        private ISeries<double> pricel { get; set; }

        private ISeries<double> pricec { get; set; }

        [Input]
        public int stochlength { get; set; }

        [Input]
        public int smoothinglength1 { get; set; }

        [Input]
        public int smoothinglength2 { get; set; }

        [Input]
        public int smoothingtype { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        protected override void Create(){
            m_stochastic1 = new Stochastic(this);
            m_ofastk = new VariableSeries<Double>(this);
            m_ofastd = new VariableSeries<Double>(this);
            m_oslowk = new VariableSeries<Double>(this);
            m_oslowd = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("SlowK", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("SlowD", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            priceh = Bars.High;
            pricel = Bars.Low;
            pricec = Bars.Close;
            m_stochastic1.priceh = priceh;
            m_stochastic1.pricel = pricel;
            m_stochastic1.pricec = pricec;
            m_stochastic1.stochlength = stochlength;
            m_stochastic1.length1 = smoothinglength1;
            m_stochastic1.length2 = smoothinglength2;
            m_stochastic1.smoothingtype = smoothingtype;
            m_stochastic1.ofastk = m_ofastk;
            m_stochastic1.ofastd = m_ofastd;
            m_stochastic1.oslowk = m_oslowk;
            m_stochastic1.oslowd = m_oslowd;
        }


        protected override void CalcBar(){
            m_stochastic1.Call();
            Plot1.Set(0, m_oslowk.Value);
            Plot2.Set(0, m_oslowd.Value);
            Plot3.Set(0, overbought);
            Plot4.Set(0, oversold);
            if (Bars.CurrentBar > 2){
                if ((this.CrossesOver(m_oslowk, m_oslowd) &&
                     PublicFunctions.DoubleLess(m_oslowk.Value, oversold))){
                    Alerts.Alert("SlowK crossing over SlowD");
                }
                else{
                    if ((this.CrossesUnder(m_oslowk, m_oslowd) &&
                         PublicFunctions.DoubleGreater(m_oslowk.Value, overbought))){
                        Alerts.Alert("SlowK crossing under SlowD");
                    }
                }
            }
        }
    }
}