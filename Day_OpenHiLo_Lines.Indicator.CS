using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Day_OpenHiLo_Lines : IndicatorObject
    {
        private VariableObject<Boolean> m_haveprevlines;

        private VariableObject<ITrendLineObject> m_tlopen;

        private VariableObject<ITrendLineObject> m_tlhigh;

        private VariableObject<ITrendLineObject> m_tllow;

        private VariableObject<Double> m_dayopen;

        private VariableObject<Double> m_dayhigh;

        private VariableObject<Double> m_daylow;

        private VariableObject<DateTime> m_currbartime;

        private VariableObject<DateTime> m_lastdiffbartime;

        public Day_OpenHiLo_Lines(object ctx) :
            base(ctx){
            datanum = 1;
            lowcolor = Color.Cyan;
            highcolor = Color.Yellow;
            opencolor = Color.Red;
        }

        [Input]
        public Color opencolor { get; set; }

        [Input]
        public Color highcolor { get; set; }

        [Input]
        public Color lowcolor { get; set; }

        [Input]
        public double datanum { get; set; }

        protected override void Create(){
            m_haveprevlines = new VariableObject<Boolean>(this);
            m_tlopen = new VariableObject<ITrendLineObject>(this);
            m_tlhigh = new VariableObject<ITrendLineObject>(this);
            m_tllow = new VariableObject<ITrendLineObject>(this);
            m_dayopen = new VariableObject<Double>(this);
            m_dayhigh = new VariableObject<Double>(this);
            m_daylow = new VariableObject<Double>(this);
            m_currbartime = new VariableObject<DateTime>(this);
            m_lastdiffbartime = new VariableObject<DateTime>(this);
        }

        protected override void StartCalc(){
            m_haveprevlines.DefaultValue = false;
            m_tlopen.DefaultValue = null;
            m_tlhigh.DefaultValue = null;
            m_tllow.DefaultValue = null;
            m_dayopen.DefaultValue = 0;
            m_dayhigh.DefaultValue = 0;
            m_daylow.DefaultValue = 0;
            m_currbartime.DefaultValue = DateTime.MinValue;
            m_lastdiffbartime.DefaultValue = DateTime.MinValue;
        }


        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;

            if ( resolution < EResolution.Day || EResolution.Volume == resolution ||
                EResolution.Second == resolution || resolution > EResolution.Quarter )
            {
                if (((Bars.TimeValue.Date != Bars.Time[1].Date)
                     && (Bars.Status == EBarState.Close))){
                    if (m_haveprevlines.Value){
                        m_tlopen.Value.End = new ChartPoint(Bars.Time[1], m_dayopen.Value);
                        m_tlhigh.Value.End = new ChartPoint(Bars.Time[1], m_dayhigh.Value);
                        m_tllow.Value.End = new ChartPoint(Bars.Time[1], m_daylow.Value);
                        m_tlopen.Value.ExtRight = false;
                        m_tlhigh.Value.ExtRight = false;
                        m_tllow.Value.ExtRight = false;
                    }
                    m_dayopen.Value = Bars.Open[0];
                    m_dayhigh.Value = Bars.High[0];
                    m_daylow.Value = Bars.Low[0];
                    m_currbartime.Value = Bars.Time[0];
                    m_lastdiffbartime.Value = Bars.Time[1];
                    m_tlopen.Value = DrwTrendLine.Create(new ChartPoint(Bars.Time[1], m_dayopen.Value),
                                                         new ChartPoint(Bars.Time[0], m_dayopen.Value));
                    m_tlhigh.Value = DrwTrendLine.Create(new ChartPoint(Bars.Time[1], m_dayhigh.Value),
                                                         new ChartPoint(Bars.Time[0], m_dayhigh.Value));
                    m_tllow.Value = DrwTrendLine.Create(new ChartPoint(Bars.Time[1], m_daylow.Value),
                                                        new ChartPoint(Bars.Time[0], m_daylow.Value));
                    m_tlopen.Value.Color = opencolor;
                    m_tlhigh.Value.Color = highcolor;
                    m_tllow.Value.Color = lowcolor;
                    m_tlopen.Value.ExtLeft = false;
                    m_tlhigh.Value.ExtLeft = false;
                    m_tllow.Value.ExtLeft = false;
                    m_tlopen.Value.ExtRight = true;
                    m_tlhigh.Value.ExtRight = true;
                    m_tllow.Value.ExtRight = true;
                    if ((m_haveprevlines.Value == false)){
                        m_haveprevlines.Value = true;
                    }
                }
                else{
                    if (m_haveprevlines.Value){
                        if ((Bars.Time[0] != m_currbartime.Value)){
                            m_lastdiffbartime.Value = m_currbartime.Value;
                            m_currbartime.Value = Bars.Time[0];
                        }
                        if (PublicFunctions.DoubleGreater(Bars.High[0], m_dayhigh.Value)){
                            m_dayhigh.Value = Bars.High[0];
                            m_tlhigh.Value.End = new ChartPoint(Bars.Time[0], m_dayhigh.Value);
                            m_tlhigh.Value.Begin = new ChartPoint(m_lastdiffbartime.Value, m_dayhigh.Value);
                        }
                        if (PublicFunctions.DoubleLess(Bars.Low[0], m_daylow.Value)){
                            m_daylow.Value = Bars.Low[0];
                            m_tllow.Value.End = new ChartPoint(Bars.Time[0], m_daylow.Value);
                            m_tllow.Value.Begin = new ChartPoint(m_lastdiffbartime.Value, m_daylow.Value);
                        }
                    }
                }
            }
            else{
                PublicFunctions.RaiseRunTimeError("Day Open-Hi-Lo Lines requires intraday bars.");
            }
        }
    }
}