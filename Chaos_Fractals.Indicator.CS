using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Chaos_Fractals : IndicatorObject
    {
        private SwingHigh m_swinghigh1;
        private SwingLow m_swinglow1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public Chaos_Fractals(object ctx) :
            base(ctx){
            strength = 2;
        }

        [Input]
        public int strength { get; set; }

        protected override void Create(){
            m_swinghigh1 = new SwingHigh(this);
            m_swinglow1 = new SwingLow(this);
            Plot1 =
                AddPlot(new PlotAttributes("BuyFractal", EPlotShapes.Point,
                                           Color.Lime, Color.Empty, 1, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("SellFractal", EPlotShapes.Point,
                                           ColorTranslator.FromWin32(15224816), Color.Empty, 1,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_swinghigh1.instance = 1;
            m_swinghigh1.price = Bars.High;
            m_swinghigh1.strength = strength;
            m_swinghigh1.length = 80;
            m_swinglow1.instance = 1;
            m_swinglow1.price = Bars.Low;
            m_swinglow1.strength = strength;
            m_swinglow1.length = 80;
        }


        protected override void CalcBar(){
            var m_value1 = m_swinghigh1[0];
            var m_value2 = m_swinglow1[0];
            if (PublicFunctions.DoubleGreater(m_value1, -1)){
                Plot1.Set(0, m_value1);
            }
            if (PublicFunctions.DoubleGreater(m_value2, -1)){
                Plot2.Set(0, m_value2);
            }
        }
    }
}