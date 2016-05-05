using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Exponential : IndicatorObject
    {
        private XAverage m_xaverage1;

        private VariableSeries<Double> m_avgexp;

        private IPlotObject Plot1;

        public Mov_Avg_Exponential(object ctx) :
            base(ctx){
            length = 9;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_xaverage1 = new XAverage(this);
            m_avgexp = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("AvgExp", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_xaverage1.Price = price;
            m_xaverage1.Length = length;
        }


        protected override void CalcBar(){
            m_avgexp.Value = m_xaverage1[0];
            if (((displace >= 0)
                 || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_avgexp.Value);
                if ((displace <= 0)){
                    if (((PublicFunctions.DoubleGreater(price[0], m_avgexp.Value) &&
                          PublicFunctions.DoubleGreater(m_avgexp.Value, m_avgexp[1]))
                         && PublicFunctions.DoubleLessEquals(m_avgexp[1], m_avgexp[2]))){
                        Alerts.Alert("Indicator turning up");
                    }
                    else{
                        if (((PublicFunctions.DoubleLess(price[0], m_avgexp.Value) &&
                              PublicFunctions.DoubleLess(m_avgexp.Value, m_avgexp[1]))
                             && PublicFunctions.DoubleGreaterEquals(m_avgexp[1], m_avgexp[2]))){
                            Alerts.Alert("Indicator turning down");
                        }
                    }
                }
            }
        }
    }
}