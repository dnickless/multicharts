using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Time_Series_Forecast : IndicatorObject
    {
        private TimeSeriesForecast m_timeseriesforecast1;

        private VariableSeries<Double> m_tsf;

        private IPlotObject Plot1;

        public Time_Series_Forecast(object ctx) :
            base(ctx){
            barsplus = 7;
            length = 9;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int barsplus { get; set; }

        protected override void Create(){
            m_timeseriesforecast1 = new TimeSeriesForecast(this);
            m_tsf = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("TSF", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
        }

        protected override void StartCalc(){
            m_timeseriesforecast1.length = length;
            m_timeseriesforecast1.tgtbar = barsplus;
        }


        protected override void CalcBar(){
            m_tsf.Value = m_timeseriesforecast1[0];
            Plot1.Set(0, m_tsf.Value);
            if (((PublicFunctions.DoubleGreater(Bars.Close[0], m_tsf.Value) &&
                  PublicFunctions.DoubleGreater(m_tsf.Value, m_tsf[1]))
                 && PublicFunctions.DoubleLessEquals(m_tsf[1], m_tsf[2]))){
                Alerts.Alert("Indicator turning up");
            }
            else{
                if (((PublicFunctions.DoubleLess(Bars.Close[0], m_tsf.Value) &&
                      PublicFunctions.DoubleLess(m_tsf.Value, m_tsf[1]))
                     && PublicFunctions.DoubleGreaterEquals(m_tsf[1], m_tsf[2]))){
                    Alerts.Alert("Indicator turning down");
                }
            }
        }
    }
}