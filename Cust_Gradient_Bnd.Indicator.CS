using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Cust_Gradient_Bnd : IndicatorObject
    {
        private FastK m_fastk1;

        private IPlotObject Plot1;
        private IPlotObject Plot2;
        private IPlotObject Plot3;
        private IPlotObject Plot4;

        public Cust_Gradient_Bnd(object ctx) :
            base(ctx){
            dncolor = Color.Magenta;
            upcolor = Color.Cyan;
            upperbound = 100;
        }

        [Input]
        public double lowerbound { get; set; }

        [Input]
        public double upperbound { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_fastk1 = new FastK(this);
            Plot1 =
                AddPlot(new PlotAttributes("Plot1", EPlotShapes.BarHigh,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("Plot2", EPlotShapes.BarLow,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("Plot3", EPlotShapes.LeftTick,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("Plot4", EPlotShapes.RightTick,
                                           Color.White, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_fastk1.stochlength = 14;
        }

        protected override void CalcBar(){
            var m_formula = m_fastk1[0];
            var m_colorlevel = PublicFunctions.GradientColor(m_formula, lowerbound, upperbound, dncolor, upcolor);
            Plot1.Set(0, Bars.High[0], m_colorlevel);
            Plot2.Set(0, Bars.Low[0], m_colorlevel);
            Plot3.Set(0, Bars.Open[0], m_colorlevel);
            Plot4.Set(0, Bars.Close[0], m_colorlevel);
        }
    }
}