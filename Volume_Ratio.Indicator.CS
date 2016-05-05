using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Volume_Ratio : IndicatorObject
    {
        private XAverage m_xaverage1;

        private NormGradientColor m_normgradientcolor1;

        private VariableSeries<Double> m_volratio;
        private VariableSeries<Double> m_volratioavg;

        private IPlotObject Plot1;
        private IPlotObject Plot2;

        public Volume_Ratio(object ctx) :
            base(ctx){
            dncolor = Color.Cyan;
            upcolor = Color.Yellow;
            colornormlength = 14;
            dnvolalertpct = (-1*5);
            upvolalertpct = 5;
            avglength = 14;
        }

        [Input]
        public int avglength { get; set; }

        [Input]
        public double upvolalertpct { get; set; }

        [Input]
        public double dnvolalertpct { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_xaverage1 = new XAverage(this);
            m_normgradientcolor1 = new NormGradientColor(this);
            m_volratio = new VariableSeries<Double>(this);
            m_volratioavg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("VolRatioAvg", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 7, 7, true));
        }

        protected override void StartCalc(){
            m_xaverage1.Price = m_volratio;
            m_xaverage1.Length = avglength;
            m_normgradientcolor1.dataseriesvalue = m_volratioavg;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
        }


        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;

            if ( resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter )
            {
                var m_totalticks = (Bars.UpTicks[0] + Bars.DownTicks[0]);
                if (PublicFunctions.DoubleGreater(m_totalticks, 0))
                {
                    m_volratio.Value = ((100
                                         *(Bars.UpTicks[0] - Bars.DownTicks[0]))
                                        /((m_totalticks)));
                }
                else
                {
                    m_volratio.Value = 0;
                }
                m_volratioavg.Value = m_xaverage1[0];
                Plot1.Set(0, m_volratioavg.Value);
                Plot2.Set(0, 0);
                Plot1.Colors[0] = m_normgradientcolor1[0];
                if (this.CrossesOver(m_volratioavg, upvolalertpct))
                {
                    Alerts.Alert("UpVol alert");
                }
                else
                {
                    if (this.CrossesUnder(m_volratioavg, dnvolalertpct))
                    {
                        Alerts.Alert("DnVol alert");
                    }
                }
            }
        }
    }
}