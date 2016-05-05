using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Standard_Error_Bands : IndicatorObject
    {
        private LinearRegValue m_linearregvalue1;

        private StdError m_stderror1;

        private VariableSeries<Double> m_linregvalue;
        private VariableSeries<Double> m_lowerband;
        private VariableSeries<Double> m_upperband;

        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private IPlotObject Plot3;

        public Standard_Error_Bands(object ctx) :
            base(ctx){
            smoothinglength = 3;
            linreglength = 21;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int linreglength { get; set; }

        [Input]
        public int smoothinglength { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_linearregvalue1 = new LinearRegValue(this);
            m_stderror1 = new StdError(this);
            m_linregvalue = new VariableSeries<Double>(this);
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("MidLine", 0, Color.Gray,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_linearregvalue1.price = price;
            m_linearregvalue1.length = linreglength;
            m_linearregvalue1.tgtbar = 0;
            m_stderror1.price = price;
            m_stderror1.length = linreglength;
        }


        protected override void CalcBar(){
            m_linregvalue.Value = m_linearregvalue1[0];
            var m_linregavg = m_linregvalue.Average(smoothinglength);
            var m_serroravg = m_stderror1.Average(smoothinglength);
            m_lowerband.Value = (m_linregavg - m_serroravg);
            m_upperband.Value = (m_linregavg + m_serroravg);
            if ((((displace >= 0) || Bars.CurrentBar > Math.Abs(displace))
                 && Bars.CurrentBar >= smoothinglength)){
                Plot1.Set(displace, m_upperband.Value);
                Plot2.Set(displace, m_lowerband.Value);
                Plot3.Set(displace, m_linregavg);
                if ((displace <= 0)){
                    if (this.CrossesOver(price, m_lowerband)){
                        Alerts.Alert("Price crossing over lower price band");
                    }
                    else{
                        if (this.CrossesUnder(price, m_upperband)){
                            Alerts.Alert("Price crossing under upper price band");
                        }
                    }
                }
            }
        }
    }
}