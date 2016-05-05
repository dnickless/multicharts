using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class MACD_Gradient : IndicatorObject
    {
        private Function.MACD m_macd1;

        private Function.XAverage m_xaverage1;

        private Function.NormGradientColor m_normgradientcolor1;

        private VariableSeries<Double> m_macdvalue;
        private VariableSeries<Double> m_macddiff;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public MACD_Gradient(object ctx) :
            base(ctx){
            dncolor = Color.Magenta;
            upcolor = Color.Yellow;
            colornormlength = 14;
            macdlength = 9;
            slowlength = 26;
            fastlength = 12;
        }

        [Input]
        public int fastlength { get; set; }

        [Input]
        public int slowlength { get; set; }

        [Input]
        public int macdlength { get; set; }

        [Input]
        public int colornormlength { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_macd1 = new Function.MACD(this);
            m_xaverage1 = new Function.XAverage(this);
            m_normgradientcolor1 = new Function.NormGradientColor(this);
            m_macdvalue = new VariableSeries<Double>(this);
            m_macddiff = new VariableSeries<Double>(this);
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
            m_macd1.Price = Bars.Close;
            m_macd1.FastLength = fastlength;
            m_macd1.SlowLength = slowlength;
            m_xaverage1.Price = m_macdvalue;
            m_xaverage1.Length = macdlength;
            m_normgradientcolor1.dataseriesvalue = m_macddiff;
            m_normgradientcolor1.crosseszero = true;
            m_normgradientcolor1.colornormlength = colornormlength;
            m_normgradientcolor1.upcolor = upcolor;
            m_normgradientcolor1.dncolor = dncolor;
        }


        protected override void CalcBar(){
            m_macdvalue.Value = m_macd1[0];
            var m_macdavg = m_xaverage1[0];
            m_macddiff.Value = (m_macdvalue.Value - m_macdavg);
            var m_colorlevel = m_normgradientcolor1[0];
            Plot1.Set(0, Bars.High[0], m_colorlevel);
            Plot2.Set(0, Bars.Low[0], m_colorlevel);
            Plot3.Set(0, Bars.Open[0], m_colorlevel);
            Plot4.Set(0, Bars.Close[0], m_colorlevel);
        }
    }
}