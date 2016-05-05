using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Adaptive : IndicatorObject
    {
        private AdaptiveMovAvg m_adaptivemovavg1;

        private VariableSeries<Double> m_maa;

        private IPlotObject Plot1;

        public Mov_Avg_Adaptive(object ctx) :
            base(ctx){
            slowavglength = 30;
            fastavglength = 2;
            effratiolength = 10;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int effratiolength { get; set; }

        [Input]
        public int fastavglength { get; set; }

        [Input]
        public int slowavglength { get; set; }

        protected override void Create(){
            m_adaptivemovavg1 = new AdaptiveMovAvg(this);
            m_maa = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MAA", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_adaptivemovavg1.price = price;
            m_adaptivemovavg1.effratiolength = effratiolength;
            m_adaptivemovavg1.fastavglength = fastavglength;
            m_adaptivemovavg1.slowavglength = slowavglength;
        }


        protected override void CalcBar(){
            m_maa.Value = m_adaptivemovavg1[0];
            Plot1.Set(0, m_maa.Value);
            if (this.CrossesOver(price, m_maa)){
                Alerts.Alert("Bullish alert");
            }
            else{
                if (this.CrossesUnder(price, m_maa)){
                    Alerts.Alert("Bearish alert");
                }
            }
        }
    }
}