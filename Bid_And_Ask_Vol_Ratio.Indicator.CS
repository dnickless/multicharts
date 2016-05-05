using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Bid_And_Ask_Vol_Ratio : IndicatorObject
    {
        private VariableSeries<Int32> m_mycurrentbar;

        private VariableSeries<Double> m_volumeatbid;

        private VariableSeries<Double> m_volumeatask;

        private VariableSeries<Double> m_bavolratio;

        private VariableSeries<Double> m_voltmp;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Bid_And_Ask_Vol_Ratio(object ctx) :
            base(ctx){
            uselog = true;
            datanum = 1;
        }

        [Input]
        public double datanum { get; set; }

        [Input]
        public bool uselog { get; set; }

        protected override void Create(){
            m_mycurrentbar = new VariableSeries<Int32>(this, 0, 0, true);
            m_volumeatbid = new VariableSeries<Double>(this, 0, 0, true);
            m_volumeatask = new VariableSeries<Double>(this, 0, 0, true);
            m_bavolratio = new VariableSeries<Double>(this, 0, 0, true);
            m_voltmp = new VariableSeries<Double>(this, 0, 0, true);
            Plot1 =
                AddPlot(new PlotAttributes("BAVolRatio", EPlotShapes.Histogram,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void CalcBar(){
            if (Bars.LastBarOnChart && Bars.Status != EBarState.Close)
            {
                var m_myvol = Bars.TrueVolume()[0];
                if (Bars.CurrentBar > m_mycurrentbar.Value)
                {
                    m_volumeatbid.Value = 0;
                    m_volumeatask.Value = 0;
                    m_bavolratio.Value = 0;
                    m_voltmp.Value = 0;
                    m_mycurrentbar.Value = Bars.CurrentBar;
                }
                if (PublicFunctions.DoubleLess(Bars.StatusLine.Bid, Bars.StatusLine.Ask)){
                    if (PublicFunctions.DoubleLessEquals(Bars.Close[0], Bars.StatusLine.Bid)){
                        m_volumeatbid.Value = ((m_volumeatbid.Value + m_myvol)
                                               - m_voltmp.Value);
                    }
                    else{
                        if (PublicFunctions.DoubleGreaterEquals(Bars.Close[0], Bars.StatusLine.Ask)){
                            m_volumeatask.Value = ((m_volumeatask.Value + m_myvol)
                                                   - m_voltmp.Value);
                        }
                    }
                }
                if ((PublicFunctions.DoubleGreater(m_volumeatbid.Value, 0) &&
                     PublicFunctions.DoubleGreater(m_volumeatask.Value, 0))){
                    m_bavolratio.Value = PublicFunctions.Iff(uselog,
                                                             Math.Log((m_volumeatask.Value/((m_volumeatbid.Value)))),
                                                             (m_volumeatask.Value/((m_volumeatbid.Value))));
                }
                m_voltmp.Value = m_myvol;
                Plot1.Set(0, m_bavolratio.Value);
            }
            Plot2.Set(0, 0);
        }
    }
}