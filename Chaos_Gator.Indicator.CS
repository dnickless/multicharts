using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Chaos_Gator : IndicatorObject
    {
        private ISeries<Double> m_value;

        private VariableSeries<Double> m_value11;

        private VariableSeries<Double> m_value12;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Chaos_Gator(object ctx) :
            base(ctx){
            valuedown = Color.Red;
            valueup = Color.Green;
            lipoffset = 3;
            lip = 5;
            teethoffset = 5;
            teeth = 8;
            jawoffset = 8;
            jaw = 13;
        }

        [Input]
        public int jaw { get; set; }

        [Input]
        public int jawoffset { get; set; }

        [Input]
        public int teeth { get; set; }

        [Input]
        public int teethoffset { get; set; }

        [Input]
        public int lip { get; set; }

        [Input]
        public int lipoffset { get; set; }

        private ISeries<Double> value{
            get { return m_value; }
        }

        [Input]
        public Color valueup { get; set; }

        [Input]
        public Color valuedown { get; set; }

        protected override void Create(){
            m_value11 = new VariableSeries<Double>(this);
            m_value12 = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("BLblue", EPlotShapes.Histogram,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("BLred", EPlotShapes.Histogram,
                                           Color.Red, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_value = new Lambda<Double>(_bb => (Bars.High[_bb] + Bars.Low[_bb])/2);
        }


        protected override void CalcBar(){
            var m_value1 = value.Average(jaw, jawoffset);
            var m_value2 = value.Average(teeth, teethoffset);
            var m_value3 = value.Average(lip, lipoffset);
            
            if (Bars.CurrentBar >= 21){
                m_value11.Value = Math.Abs((m_value1 - m_value2));
                Plot1.Set(0, m_value11.Value,
                          PublicFunctions.DoubleGreater(m_value11.Value, m_value11[1]) ? valueup : valuedown);
                
                m_value12.Value = -1*Math.Abs((m_value2 - m_value3));
                Plot2.Set(0, m_value12.Value,
                          PublicFunctions.DoubleGreater(m_value12.Value, m_value12[1]) ? valueup : valuedown);
            }
        }
    }
}