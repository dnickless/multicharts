using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Price_Channel : IndicatorObject
    {
        private VariableSeries<Double> m_lowerband;

        private VariableSeries<Double> m_upperband;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Price_Channel(object ctx) :
            base(ctx){
            length = 20;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_lowerband = new VariableSeries<Double>(this);
            m_upperband = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("UpperBand", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("LowerBand", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_lowerband.Value = Bars.Low.Lowest(length, 1);
            m_upperband.Value = Bars.High.Highest(length, 1);
            if (((displace >= 0) || Bars.CurrentBar > Math.Abs(displace))){
                Plot1.Set(displace, m_upperband.Value);
                Plot2.Set(displace, m_lowerband.Value);
                if ((displace <= 0)){
                    if (this.CrossesUnder(Bars.Low, m_lowerband)){
                        Alerts.Alert("Price making new low");
                    }
                    if (this.CrossesOver(Bars.High, m_upperband)){
                        Alerts.Alert("Price making new high");
                    }
                }
            }
        }
    }
}