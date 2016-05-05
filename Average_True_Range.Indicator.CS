using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Average_True_Range : IndicatorObject
    {
        private VariableSeries<Double> m_atr;

        private IPlotObject Plot1;

        public Average_True_Range(object ctx) :
            base(ctx){
            alertlength = 14;
            atrlength = 14;
        }

        [Input]
        public int atrlength { get; set; }

        [Input]
        public int alertlength { get; set; }

        protected override void Create(){
            m_atr = new VariableSeries<Double>(this);
            Plot1 = AddPlot(new PlotAttributes("ATR", 0, Color.Cyan, Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_atr.Value = this.AverageTrueRange(atrlength);
            Plot1.Set(0, m_atr.Value);
            if ((m_atr.HighestBar(alertlength) == 0))
            {
                Alerts.Alert("Indicator at high");
            }
            else{
                if ((m_atr.LowestBar(alertlength) == 0))
                {
                    Alerts.Alert("Indicator at low");
                }
            }
        }
    }
}