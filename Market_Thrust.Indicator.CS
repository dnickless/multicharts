using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Market_Thrust : IndicatorObject
    {
        private ISeries<Double> m_advissues;

        private ISeries<Double> m_advvol;

        private ISeries<Double> m_decissues;

        private ISeries<Double> m_decvol;

        private VariableSeries<Double> m_rawmktthr;

        private VariableSeries<Double> m_mktthr;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Market_Thrust(object ctx) :
            base(ctx){
            smoothinglength = 14;
        }

        private ISeries<Double> advissues{
            get { return m_advissues; }
        }

        private ISeries<Double> advvol{
            get { return m_advvol; }
        }

        private ISeries<Double> decissues{
            get { return m_decissues; }
        }

        private ISeries<Double> decvol{
            get { return m_decvol; }
        }

        [Input]
        public int smoothinglength { get; set; }

        protected override void Create(){
            m_rawmktthr = new VariableSeries<Double>(this);
            m_mktthr = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("RawMktThr", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("MktThr", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("ZeroLine", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_advissues = Bars.Close;
            m_advvol = BarsOfData(2).Close;
            m_decissues = BarsOfData(3).Close;
            m_decvol = BarsOfData(4).Close;
        }


        protected override void CalcBar(){
            m_rawmktthr.Value = advissues[0]*advvol[0] - decissues[0]*decvol[0];
            m_mktthr.Value = m_rawmktthr.Average(smoothinglength);
            Plot1.Set(0, m_rawmktthr.Value);
            Plot2.Set(0, m_mktthr.Value);
            Plot3.Set(0, 0);
            if (this.CrossesOver(m_mktthr, 0)){
                Alerts.Alert("Indicator turning positive");
            }
            else{
                if (this.CrossesUnder(m_mktthr, 0)){
                    Alerts.Alert("Indicator turning negative");
                }
            }
        }
    }
}