using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Volatility : IndicatorObject
    {
        private Function.Volatility m_volatility1;

        private IPlotObject Plot1;

        public Volatility(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_volatility1 = new Function.Volatility(this);
            Plot1 =
                AddPlot(new PlotAttributes("Volty", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_volatility1.length = length;
        }


        protected override void CalcBar(){
            Plot1.Set(0, m_volatility1.Value);
        }
    }
}