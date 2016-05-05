using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Mkt_Facilitation_Idx : IndicatorObject
    {
        private VariableSeries<Double> m_mktfacilx;

        private IPlotObject Plot1;

        public Mkt_Facilitation_Idx(object ctx) :
            base(ctx){
            alertlength = 14;
        }

        [Input]
        public int alertlength { get; set; }

        protected override void Create(){
            m_mktfacilx = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MktFacilX", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }


        protected override void CalcBar()
        {
            m_mktfacilx.Value = Bars.MFI();
            Plot1.Set(0, m_mktfacilx.Value);

            if (m_mktfacilx.HighestBar(alertlength) == 0)
            {
                Alerts.Alert("Indicator at high");
            }
            else{
                if (m_mktfacilx.LowestBar(alertlength) == 0){
                    Alerts.Alert("Indicator at low");
                }
            }
        }
    }
}