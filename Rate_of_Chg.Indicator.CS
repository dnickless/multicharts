using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Rate_of_Chg : IndicatorObject
    {
        private NormGradientColor m_normgradientcolor1;

        private VariableSeries<Double> m_roc;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Rate_of_Chg(object ctx) :
            base(ctx){
            gridforegroundcolor = Color.Black;
            dncolor = Color.Magenta;
            upcolor = Color.Cyan;
            colornormlength = 14;
            alertlength = 14;
            roclength = 14;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int roclength { get; set; }

        [Input]
        public int alertlength { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        [Input]
        public Color gridforegroundcolor { get; set; }

        protected override void Create(){
            m_normgradientcolor1 = new NormGradientColor(this);
            m_roc = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("ROC", 0, Color.Silver,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_normgradientcolor1.dataseriesvalue = m_roc;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
        }


        protected override void CalcBar(){

            m_roc.Value = price.RateOfChange(roclength);
            Plot1.Set(0, m_roc.Value);
            Plot2.Set(0, 0);
            var m_colorlevel = m_normgradientcolor1[0];
            if (((int)Environment.ApplicationCode == 1))
            {
                Plot1.Colors[0] = m_colorlevel;
            }
            else
            {
                if ((int)Environment.ApplicationCode > 1)
                {
                    Plot1.Colors[0] = gridforegroundcolor;
                    Plot1.BGColor = m_colorlevel;
                }
            }

            if ((m_roc.HighestBar(alertlength) == 0))
            {
                Alerts.Alert("Indicator at high");
            }
            else
            {
                if ((m_roc.LowestBar(alertlength) == 0))
                {
                    Alerts.Alert("Indicator at low");
                }
            }
        }
    }
}