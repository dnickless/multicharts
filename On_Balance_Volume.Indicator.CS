using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class On_Balance_Volume : IndicatorObject
    {
        private OBV m_obv1;

        private VariableSeries<Double> m_obvvalue;

        private IPlotObject Plot1;

        public On_Balance_Volume(object ctx) :
            base(ctx){
            alertlength = 14;
        }

        [Input]
        public int alertlength { get; set; }

        protected override void Create(){
            m_obv1 = new OBV(this);
            m_obvvalue = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("OBV", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_obvvalue.Value = m_obv1[0];
            Plot1.Set(0, m_obvvalue.Value);
            if (((Bars.Close.LowestBar(alertlength) == 0)
                 && PublicFunctions.DoubleGreater(m_obvvalue.LowestBar(alertlength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(alertlength) == 0)
                     && PublicFunctions.DoubleGreater(m_obvvalue.HighestBar(alertlength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}