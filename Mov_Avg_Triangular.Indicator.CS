using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Triangular : IndicatorObject
    {
        private TriAverage m_triaverage1;

        private VariableSeries<Double> m_avgtri;

        private IPlotObject Plot1;

        public Mov_Avg_Triangular(object ctx) :
            base(ctx){
            length = 9;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_triaverage1 = new TriAverage(this);
            m_avgtri = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("AvgTri", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_triaverage1.price = price;
            m_triaverage1.length = length;
        }


        protected override void CalcBar(){
            m_avgtri.Value = m_triaverage1[0];
            if (((displace >= 0) || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_avgtri.Value);
                if ((displace <= 0)){
                    if (((PublicFunctions.DoubleGreater(price[0], m_avgtri.Value) &&
                          PublicFunctions.DoubleGreater(m_avgtri.Value, m_avgtri[1]))
                         && PublicFunctions.DoubleLessEquals(m_avgtri[1], m_avgtri[2]))){
                        Alerts.Alert("Indicator turning up");
                    }
                    else{
                        if (((PublicFunctions.DoubleLess(price[0], m_avgtri.Value) &&
                              PublicFunctions.DoubleLess(m_avgtri.Value, m_avgtri[1]))
                             && PublicFunctions.DoubleGreaterEquals(m_avgtri[1], m_avgtri[2]))){
                            Alerts.Alert("Indicator turning down");
                        }
                    }
                }
            }
        }
    }
}