using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class ADXR : IndicatorObject
    {
        private DirMovement m_dirmovement1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public ADXR(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_dirmovement1 = new DirMovement(this);
            Plot1 =
                AddPlot(new PlotAttributes("ADX", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("ADXR", 0, Color.Magenta,
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
            Plot1.Set(0, m_dirmovement1.ADX.Value);
            Plot2.Set(0, m_dirmovement1.ADXR.Value);

            if (PublicFunctions.DoubleGreater(m_dirmovement1.ADXR.Value, m_dirmovement1.ADXR[1]) 
                &&PublicFunctions.DoubleLessEquals(m_dirmovement1.ADXR[1], m_dirmovement1.ADXR[2]))
            {
                Alerts.Alert("ADXR turning up");
            }
            else{
                if (PublicFunctions.DoubleLess(m_dirmovement1.ADXR.Value, m_dirmovement1.ADXR[1]) 
                    &&PublicFunctions.DoubleGreaterEquals(m_dirmovement1.ADXR[1], m_dirmovement1.ADXR[2]))
                {
                    Alerts.Alert("ADXR turning down");
                }
            }
        }
    }
}