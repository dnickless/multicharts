using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Ulcer_Index : IndicatorObject
    {
        private VariableSeries<Double> m_ulcerx;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Ulcer_Index(object ctx) :
            base(ctx){
            safelevel = 5;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double safelevel { get; set; }

        protected override void Create(){
            m_ulcerx = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("UlcerX", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("SafeLevel", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            m_ulcerx.Value = Bars.Close.UlcerIndex(length);
            Plot1.Set(0, m_ulcerx.Value);
            Plot2.Set(0, safelevel);
            if (this.CrossesOver(m_ulcerx, safelevel)){
                Alerts.Alert("Indicator crossing over Safe Level");
            }
        }
    }
}