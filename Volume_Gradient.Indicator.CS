using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Volume_Gradient : IndicatorObject
    {
        private NormGradientColor m_normgradientcolor1;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Volume_Gradient(object ctx) :
            base(ctx){
            dncolor = Color.Magenta;
            upcolor = Color.Yellow;
            colornormlength = 14;
        }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_normgradientcolor1 = new NormGradientColor(this);
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
            m_normgradientcolor1.dataseriesvalue = Bars.TrueVolume();
            m_normgradientcolor1.crosseszero = false;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
        }


        protected override void CalcBar()
        {
            var m_colorlevel = m_normgradientcolor1[0];
            Plot1.Set(0, Bars.High[0], m_colorlevel);
            Plot2.Set(0, Bars.Low[0], m_colorlevel);
            Plot3.Set(0, Bars.Open[0], m_colorlevel);
            Plot4.Set(0, Bars.Close[0], m_colorlevel);
        }
    }
}