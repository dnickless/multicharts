using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Volatility_Std_Dev : IndicatorObject
    {
        private VolatilityStdDev m_volatilitystddev1;

        private IPlotObject Plot1;

        public Volatility_Std_Dev(object ctx) :
            base(ctx){
            length = 30;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_volatilitystddev1 = new VolatilityStdDev(this);
            Plot1 =
                AddPlot(new PlotAttributes("VoltySDev", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_volatilitystddev1.numdays = length;
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_volatilitystddev1[0]);
        }
    }
}