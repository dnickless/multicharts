using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class OHLC_Yesterday : IndicatorObject
    {
        private VariableObject<Int32> m_counter;

        private VariableObject<Double> m_yestopen;

        private VariableObject<Double> m_todaysopen;

        private VariableObject<Double> m_yesthigh;

        private VariableObject<Double> m_todayshigh;

        private VariableObject<Double> m_yestlow;

        private VariableObject<Double> m_todayslow;

        private VariableObject<Double> m_yestclose;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public OHLC_Yesterday(object ctx) :
            base(ctx) {}

        protected override void Create(){
            m_counter = new VariableObject<Int32>(this);
            m_yestopen = new VariableObject<Double>(this);
            m_todaysopen = new VariableObject<Double>(this);
            m_yesthigh = new VariableObject<Double>(this);
            m_todayshigh = new VariableObject<Double>(this);
            m_yestlow = new VariableObject<Double>(this);
            m_todayslow = new VariableObject<Double>(this);
            m_yestclose = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("YestOpen", EPlotShapes.LeftTick,
                                           Color.Magenta, Color.Empty, 2,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("YestHigh", EPlotShapes.LeftTick,
                                           Color.Blue, Color.Empty, 2, 0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("YestLow", EPlotShapes.LeftTick,
                                           Color.Yellow, Color.Empty, 2,
                                           0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("YestClose", EPlotShapes.LeftTick,
                                           Color.Cyan, Color.Empty, 2, 0,
                                           true));
        }

        protected override void CalcBar()
        {
            EResolution resolution = Bars.Info.Resolution.Type;
            if (resolution == EResolution.Quarter ||
                EResolution.Week <= resolution && resolution <= EResolution.Year) return;

            if (Bars.Time[0].Date != Bars.Time[1].Date)
            {
                m_counter.Value = (m_counter.Value + 1);
                m_yestopen.Value = m_todaysopen.Value;
                m_yesthigh.Value = m_todayshigh.Value;
                m_yestlow.Value = m_todayslow.Value;
                m_yestclose.Value = Bars.Close[1];
                m_todaysopen.Value = Bars.Open[0];
                m_todayshigh.Value = Bars.High[0];
                m_todayslow.Value = Bars.Low[0];
            }
            else{
                if (PublicFunctions.DoubleGreater(Bars.High[0], m_todayshigh.Value)){
                    m_todayshigh.Value = Bars.High[0];
                }
                if (PublicFunctions.DoubleLess(Bars.Low[0], m_todayslow.Value)){
                    m_todayslow.Value = Bars.Low[0];
                }
            }

            if (m_counter.Value >= 2)
            {
                Plot1.Set(0, m_yestopen.Value);
                Plot2.Set(0, m_yesthigh.Value);
                Plot3.Set(0, m_yestlow.Value);
                Plot4.Set(0, m_yestclose.Value);
            }
        }
    }
}