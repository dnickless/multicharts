using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Mov_Avg_Exp_Ribbon : IndicatorObject
    {
        private ArraySimple<Int32> m_length;

        private ArraySimple<Double> m_smoothingfactor;

        private ArraySeries<Double> m_xma;

        private ArraySimple<Color> m_color;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        private IPlotObject Plot5;

        private IPlotObject Plot6;

        private IPlotObject Plot7;

        private IPlotObject Plot8;

        public Mov_Avg_Exp_Ribbon(object ctx) :
            base(ctx){
            slowcolor = Color.Red;
            fastcolor = Color.Yellow;
            incrementormultiplier = 10;
            arithorgeom_1or2 = 1;
            baselength = 10;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public int baselength { get; set; }

        [Input]
        public int arithorgeom_1or2 { get; set; }

        [Input]
        public int incrementormultiplier { get; set; }

        [Input]
        public Color fastcolor { get; set; }

        [Input]
        public Color slowcolor { get; set; }

        protected override void Create(){
            m_length = new ArraySimple<Int32>(this, 9);
            m_smoothingfactor = new ArraySimple<Double>(this, 9);
            m_xma = new ArraySeries<Double>(this, 9);
            m_color = new ArraySimple<Color>(this, 9);
            Plot1 =
                AddPlot(new PlotAttributes("XMA1", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("XMA2", 0, Color.Red,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("XMA3", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("XMA4", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
            Plot5 =
                AddPlot(new PlotAttributes("XMA5", 0, Color.Blue,
                                           Color.Empty,
                                           0, 0, true));
            Plot6 =
                AddPlot(new PlotAttributes("XMA6", 0, Color.Red,
                                           Color.Empty, 0, 0, true));
            Plot7 =
                AddPlot(new PlotAttributes("XMA7", 0, Color.Cyan,
                                           Color.Empty, 0, 0, true));
            Plot8 =
                AddPlot(new PlotAttributes("XMA8", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            price = Bars.Close;
        }

        protected override void CalcBar(){
            if ((Bars.CurrentBar == 1)){
                m_length[0][((1))] = baselength;
                m_smoothingfactor[0][((1))] = (2/((double) ((m_length[0][((1))] + 1))));
                m_xma[0][((1))] = price[0];
                m_color[0][((1))] = PublicFunctions.GradientColor(1, 1, 8, fastcolor, slowcolor);
                for (var m_counter = 1; (m_counter <= 7); m_counter = (m_counter + 1)){
                    if ((arithorgeom_1or2 == 1)){
                        m_length[0][(((m_counter + 1)))] = (m_length[0][((m_counter))] + incrementormultiplier);
                    }
                    else{
                        m_length[0][(((m_counter + 1)))] = (m_length[0][((m_counter))]*incrementormultiplier);
                    }
                    m_smoothingfactor[0][(((m_counter + 1)))] = (2/((double) ((m_length[0][(((m_counter + 1)))] + 1))));
                    m_xma[0][(((m_counter + 1)))] = price[0];
                    m_color[0][(((m_counter + 1)))] = PublicFunctions.GradientColor((m_counter + 1), 1, 8, fastcolor,
                                                                                    slowcolor);
                }
            }
            else{
                for (var m_counter = 1; (m_counter <= 8); m_counter = (m_counter + 1)){
                    m_xma[0][((m_counter))] = (m_xma[1][((m_counter))]
                                               + (m_smoothingfactor[0][((m_counter))]
                                                  *(price[0] - m_xma[1][((m_counter))])));
                }
            }
            Plot1.Set(0, m_xma[0][((1))], m_color[0][((1))]);
            Plot2.Set(0, m_xma[0][((2))], m_color[0][((2))]);
            Plot3.Set(0, m_xma[0][((3))], m_color[0][((3))]);
            Plot4.Set(0, m_xma[0][((4))], m_color[0][((4))]);
            Plot5.Set(0, m_xma[0][((5))], m_color[0][((5))]);
            Plot6.Set(0, m_xma[0][((6))], m_color[0][((6))]);
            Plot7.Set(0, m_xma[0][((7))], m_color[0][((7))]);
            Plot8.Set(0, m_xma[0][((8))], m_color[0][((8))]);
        }
    }
}