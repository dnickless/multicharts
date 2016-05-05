using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Chaos_Alligator : IndicatorObject
    {
        private ISeries<Double> m_avg_price;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Chaos_Alligator(object ctx) :
            base(ctx){
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

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("BLblue", 0, Color.Red,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("BLred", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("BLgreen", 0, Color.Lime,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_avg_price = new Lambda<Double>(_bb => ((Bars.High[_bb] + Bars.Low[_bb])/((2))));
        }


        protected override void CalcBar(){
            var m_value1 = m_avg_price.Average(jaw, jawoffset);
            var m_value2 = m_avg_price.Average(teeth, teethoffset);
            var m_value3 = m_avg_price.Average(lip, lipoffset);
            if (Bars.CurrentBar >= 21){
                Plot1.Set(0, m_value1);
                Plot2.Set(0, m_value2);
                Plot3.Set(0, m_value3);
            }
        }
    }
}