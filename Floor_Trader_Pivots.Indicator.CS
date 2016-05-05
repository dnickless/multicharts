using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Floor_Trader_Pivots : IndicatorObject
    {
        private VariableObject<Double> m_s1;

        private VariableObject<Double> m_s2;

        private VariableObject<Double> m_s3;

        private VariableObject<Double> m_r1;

        private VariableObject<Double> m_r2;

        private VariableObject<Double> m_r3;

        private VariableObject<Double> m_pp;

        private VariableObject<Double> m_todayshigh;

        private VariableObject<Double> m_todayslow;

        private VariableObject<Int32> m_counter;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        private IPlotObject Plot5;

        private IPlotObject Plot6;

        private IPlotObject Plot7;

        public Floor_Trader_Pivots(object ctx) :
            base(ctx){
            plot_5or7 = 5;
        }

        [Input]
        public int plot_5or7 { get; set; }

        protected override void Create(){
            m_s1 = new VariableObject<Double>(this);
            m_s2 = new VariableObject<Double>(this);
            m_s3 = new VariableObject<Double>(this);
            m_r1 = new VariableObject<Double>(this);
            m_r2 = new VariableObject<Double>(this);
            m_r3 = new VariableObject<Double>(this);
            m_pp = new VariableObject<Double>(this);
            m_todayshigh = new VariableObject<Double>(this);
            m_todayslow = new VariableObject<Double>(this);
            m_counter = new VariableObject<Int32>(this);
            Plot1 =
                AddPlot(new PlotAttributes("R3", EPlotShapes.LeftTick,
                                           Color.Magenta, Color.Empty, 2,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("R2", EPlotShapes.LeftTick, Color.Blue,
                                           Color.Empty, 2, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("R1", EPlotShapes.LeftTick,
                                           Color.Yellow, Color.Empty, 2,
                                           0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("PP", EPlotShapes.LeftTick, Color.Cyan,
                                           Color.Empty, 2, 0, true));
            Plot5 =
                AddPlot(new PlotAttributes("S1", EPlotShapes.LeftTick,
                                           Color.Yellow, Color.Empty, 2,
                                           0,
                                           true));
            Plot6 =
                AddPlot(new PlotAttributes("S2", EPlotShapes.LeftTick, Color.Blue,
                                           Color.Empty, 2, 0, true));
            Plot7 =
                AddPlot(new PlotAttributes("S3", EPlotShapes.LeftTick,
                                           Color.Magenta, Color.Empty, 2,
                                           0,
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
                var m_yesthigh = m_todayshigh.Value;
                var m_yestlow = m_todayslow.Value;
                var m_yestclose = Bars.Close[1];
                m_todayshigh.Value = Bars.High[0];
                m_todayslow.Value = Bars.Low[0];
                m_pp.Value = (((m_yesthigh + m_yestlow)
                               + m_yestclose)
                              /((3)));
                m_r1.Value = ((m_pp.Value*2)
                              - m_yestlow);
                m_r2.Value = ((m_pp.Value + m_yesthigh)
                              - m_yestlow);
                m_r3.Value = ((m_r2.Value + m_yesthigh)
                              - m_yestlow);
                m_s1.Value = ((m_pp.Value*2)
                              - m_yesthigh);
                m_s2.Value = ((m_pp.Value - m_yesthigh)
                              + m_yestlow);
                m_s3.Value = ((m_s2.Value - m_yesthigh)
                              + m_yestlow);
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
                if ((plot_5or7 == 7))
                {
                    Plot1.Set(0, m_r3.Value);
                }
                Plot2.Set(0, m_r2.Value);
                Plot3.Set(0, m_r1.Value);
                Plot4.Set(0, m_pp.Value);
                Plot5.Set(0, m_s1.Value);
                Plot6.Set(0, m_s2.Value);
                if ((plot_5or7 == 7))
                {
                    Plot7.Set(0, m_s3.Value);
                }
            }
        }
    }
}