using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Volume_Avg : IndicatorObject
    {
        private AverageFC m_averagefc1;

        private VariableSeries<Double> m_vvol;
        private VariableSeries<Double> m_avgvvol;

        private double m_alertfactor;
        private string m_alertstr;

        private IPlotObject Plot1;
        private IPlotObject Plot2;

        public Volume_Avg(object ctx) :
            base(ctx){
            downcolor = Color.Red;
            upcolor = Color.Cyan;
            alertpct = 50;
            avglength = 50;
        }

        [Input]
        public int avglength { get; set; }

        [Input]
        public double alertpct { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color downcolor { get; set; }

        private ISeries<double> Volume { get; set; }

        protected override void Create(){
            m_averagefc1 = new AverageFC(this);
            m_vvol = new VariableSeries<Double>(this);
            m_avgvvol = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Vol", EPlotShapes.Histogram,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("VolAvg", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            Volume = Bars.TrueVolume();
            
            m_averagefc1.price = Volume;
            m_averagefc1.length = avglength;

            m_alertfactor = (1 + (alertpct*0.01));
            m_alertstr = Math.Round(alertpct, 2).ToString();
        }

        protected override void CalcBar()
        {
            m_vvol.Value = Volume[0];
            m_avgvvol.Value = m_averagefc1[0];
            Plot1.Set(0, m_vvol.Value);
            Plot2.Set(0, m_avgvvol.Value);
            if (this.CrossesOver(m_vvol, (m_avgvvol.Value * m_alertfactor)))
            {
                Alerts.Alert((("Volume breaking through " + m_alertstr)
                              + "% above its avg"));
            }

            if (PublicFunctions.DoubleGreater(Bars.Close[0], Bars.Close[1])){
                Plot1.Colors[0] = upcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(Bars.Close[0], Bars.Close[1])){
                    Plot1.Colors[0] = downcolor;
                }
            }
        }
    }
}