using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Weighted : IndicatorObject
    {
        private VariableSeries<Double> m_avgwtd;

        private IPlotObject Plot1;

        public Mov_Avg_Weighted(object ctx) :
            base(ctx){
            length = 9;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_avgwtd = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("AvgWtd", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
        }


        protected override void CalcBar(){
            if (((displace >= 0)
                 || Bars.CurrentBar > Math.Abs(displace))){
                     m_avgwtd.Value = price.WAverageCustom(length);
                Plot1.Set(displace, m_avgwtd.Value);
                if ((displace <= 0)){
                    if (((PublicFunctions.DoubleGreater(price[0], m_avgwtd.Value) &&
                          PublicFunctions.DoubleGreater(m_avgwtd.Value, m_avgwtd[1]))
                         && PublicFunctions.DoubleLessEquals(m_avgwtd[1], m_avgwtd[2]))){
                        Alerts.Alert("Indicator turning up");
                    }
                    else{
                        if (((PublicFunctions.DoubleLess(price[0], m_avgwtd.Value) &&
                              PublicFunctions.DoubleLess(m_avgwtd.Value, m_avgwtd[1]))
                             && PublicFunctions.DoubleGreaterEquals(m_avgwtd[1], m_avgwtd[2]))){
                            Alerts.Alert("Indicator turning down");
                        }
                    }
                }
            }
        }
    }
}