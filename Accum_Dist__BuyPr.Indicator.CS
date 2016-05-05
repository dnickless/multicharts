using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Accum_Dist__BuyPr : IndicatorObject
    {
        private LWAccDis m_lwaccdis1;

        private VariableSeries<Double> m_accdstbuypr;

        private IPlotObject Plot1;

        public Accum_Dist__BuyPr(object ctx) :
            base(ctx){
            alertlength = 14;
        }

        [Input]
        public int alertlength { get; set; }

        protected override void Create(){
            m_lwaccdis1 = new LWAccDis(this);
            m_accdstbuypr = new VariableSeries<Double>(this);
            Plot1 = AddPlot(new PlotAttributes("AccDst-BuyPr", 0, Color.Yellow, Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_accdstbuypr.Value = m_lwaccdis1[0];
            Plot1.Set(0, m_accdstbuypr.Value);
            if (((Bars.Close.LowestBar(alertlength) == 0)
                 && PublicFunctions.DoubleGreater(m_accdstbuypr.LowestBar(alertlength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(alertlength) == 0)
                     && PublicFunctions.DoubleGreater(m_accdstbuypr.HighestBar(alertlength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}