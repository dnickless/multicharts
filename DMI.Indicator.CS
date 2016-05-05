using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class DMI : IndicatorObject
    {
        private DirMovement m_dirmovement1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public DMI(object ctx) :
            base(ctx){
            adxtrend = 25;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double adxtrend { get; set; }

        protected override void Create(){
            m_dirmovement1 = new DirMovement(this);
            Plot1 =
                AddPlot(new PlotAttributes("DMI+", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("DMI-", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("ADX", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_dirmovement1.PriceH = Bars.High;
            m_dirmovement1.PriceL = Bars.Low;
            m_dirmovement1.PriceC = Bars.Close;
            m_dirmovement1.Length = length;
        }


        protected override void CalcBar(){
            m_dirmovement1.Call();
            Plot1.Set(0, m_dirmovement1.DMIPlus.Value);
            Plot2.Set(0, m_dirmovement1.DMIMinus.Value);
            Plot3.Set(0, m_dirmovement1.ADX.Value);

            if (PublicFunctions.DoubleGreater(m_dirmovement1.ADX.Value, adxtrend))
            {
                if (this.CrossesOver(m_dirmovement1.DMIPlus, m_dirmovement1.DMIMinus))
                {
                    Alerts.Alert("Bullish alert");
                }
                else{
                    if (this.CrossesUnder(m_dirmovement1.DMIPlus, m_dirmovement1.DMIMinus))
                    {
                        Alerts.Alert("Bearish alert");
                    }
                }
            }
        }
    }
}