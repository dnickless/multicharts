using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Correlation : IndicatorObject
    {
        private VariableSeries<Double> m_correlval;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Correlation(object ctx) :
            base(ctx){
            negcorralert = -0.7;
            poscorralert = 0.7;
            length = 14;
        }

        private ISeries<double> indepdata { get; set; }

        private ISeries<double> depdata { get; set; }

        [Input]
        public int length { get; set; }

        [Input]
        public double poscorralert { get; set; }

        [Input]
        public double negcorralert { get; set; }

        protected override void Create(){
            m_correlval = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Correl", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("PosCorrAlert", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("NegCorrAlert", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            indepdata = Bars.Close;
            depdata = BarsOfData(2).Close;
        }
        
        protected override void CalcBar(){
            m_correlval.Value = Function.Correlation.CalcCorrelation(indepdata, depdata, length);
            Plot1.Set(0, m_correlval.Value);
            Plot2.Set(0, poscorralert);
            Plot3.Set(0, negcorralert);
            if (this.CrossesOver(m_correlval, poscorralert)){
                Alerts.Alert("Positive correlation alert");
            }
            else{
                if (this.CrossesUnder(m_correlval, negcorralert)){
                    Alerts.Alert("Negative correlation alert");
                }
            }
        }
    }
}