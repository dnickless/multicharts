using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Ultimate_Osc : IndicatorObject
    {
        private UltimateOscillator m_ultimateoscillator1;

        private IPlotObject Plot1;

        public Ultimate_Osc(object ctx) :
            base(ctx){
            slowlength = 28;
            medlength = 14;
            fastlength = 7;
        }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int medlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        protected override void Create(){
            m_ultimateoscillator1 = new UltimateOscillator(this);
            Plot1 =
                AddPlot(new PlotAttributes("UltOsc", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_ultimateoscillator1.shortlength = fastlength;
            m_ultimateoscillator1.middllength = medlength;
            m_ultimateoscillator1.longlength = slowlength;
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_ultimateoscillator1[0]);
        }
    }
}