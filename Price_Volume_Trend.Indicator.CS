using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Price_Volume_Trend : IndicatorObject
    {
        private PriceVolTrend m_pricevoltrend1;

        private VariableSeries<Double> m_pvt;

        private IPlotObject Plot1;

        public Price_Volume_Trend(object ctx) :
            base(ctx){
            alertlength = 14;
        }

        [Input]
        public int alertlength { get; set; }

        protected override void Create(){
            m_pricevoltrend1 = new PriceVolTrend(this);
            m_pvt = new VariableSeries<Double>(this);
            Plot1 = AddPlot(new PlotAttributes("PVT", 0, Color.Yellow, Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_pvt.Value = m_pricevoltrend1[0];
            Plot1.Set(0, m_pvt.Value);
            if (((Bars.Close.LowestBar(alertlength) == 0)
                 && PublicFunctions.DoubleGreater(m_pvt.LowestBar(alertlength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(alertlength) == 0)
                     && PublicFunctions.DoubleGreater(m_pvt.HighestBar(alertlength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}