using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Head_And_Shoulders : IndicatorObject
    {
        private BarNumber m_barnumber1;

        private SwingLowBar m_swinglowbar1;

        private SwingLowBar m_swinglowbar2;

        private SwingHighBar m_swinghighbar1;

        private SwingHighBar m_swinghighbar2;

        private VariableSeries<Double> m_barnum;

        private VariableObject<Int32> m_neckswbar;

        private VariableObject<Int32> m_hnsswbar;

        private VariableSeries<Double> m_neckswprice;

        private VariableSeries<Double> m_hnsswprice;

        private VariableObject<Boolean> m_vertorder_hvss;

        private VariableObject<Boolean> m_vertorder_nvss;

        private VariableObject<Double> m_necklinelimit;

        private VariableObject<Double> m_maxnecktoheaddist;

        private VariableObject<Boolean> m_vertproportion;

        private VariableObject<Color> m_color;

        private ITrendLineObject m_tlref;

        private VariableObject<Double> m_confirmbarnum;

        private VariableObject<int> m_lastcrossedtl;

        private Array2DSimple<Double> m_neckswings;

        private Array2DSimple<Double> m_hnsswings;

        private IPlotObject Plot1;

        public Head_And_Shoulders(object ctx) :
            base(ctx){
            color2 = Color.Cyan;
            color1 = Color.Yellow;
            barspast = 10;
            drawneckline = true;
            confirmvolume = true;
            hilo = 1;
            vertproportionlimit = 0.33;
            hnsswstrength = 2;
            neckswstrength = 2;
        }

        [Input]
        public int neckswstrength { get; set; }

        [Input]
        public int hnsswstrength { get; set; }

        [Input]
        public double vertproportionlimit { get; set; }

        [Input]
        public int hilo { get; set; }

        [Input]
        public bool confirmvolume { get; set; }

        [Input]
        public bool drawneckline { get; set; }

        [Input]
        public int barspast { get; set; }

        [Input]
        public Color color1 { get; set; }

        [Input]
        public Color color2 { get; set; }

        protected override void Create(){
            m_barnumber1 = new BarNumber(this);
            m_swinglowbar1 = new SwingLowBar(this);
            m_swinglowbar2 = new SwingLowBar(this);
            m_swinghighbar1 = new SwingHighBar(this);
            m_swinghighbar2 = new SwingHighBar(this);
            m_barnum = new VariableSeries<Double>(this);
            m_neckswbar = new VariableObject<Int32>(this);
            m_hnsswbar = new VariableObject<Int32>(this);
            m_neckswprice = new VariableSeries<Double>(this);
            m_hnsswprice = new VariableSeries<Double>(this);
            m_vertorder_hvss = new VariableObject<Boolean>(this);
            m_vertorder_nvss = new VariableObject<Boolean>(this);
            m_necklinelimit = new VariableObject<Double>(this);
            m_maxnecktoheaddist = new VariableObject<Double>(this);
            m_vertproportion = new VariableObject<Boolean>(this);
            m_color = new VariableObject<Color>(this);
            m_confirmbarnum = new VariableObject<Double>(this);
            m_lastcrossedtl = new VariableObject<int>(this);
            m_neckswings = new Array2DSimple<Double>(this, 2, 4);
            m_hnsswings = new Array2DSimple<Double>(this, 3, 5);
            Plot1 =
                AddPlot(new PlotAttributes("H&S", EPlotShapes.Point, Color.White,
                                           Color.Empty, 4, 0, true));
        }

        protected override void StartCalc(){
            var mNeckswlength = (neckswstrength + 1);
            var mHnsswlength = (hnsswstrength + 1);

            m_swinglowbar1.instance = 1;
            m_swinglowbar1.price = Bars.Low;
            m_swinglowbar1.strength = neckswstrength;
            m_swinglowbar1.length = mNeckswlength;

            m_swinglowbar2.instance = 1;
            m_swinglowbar2.price = Bars.Low;
            m_swinglowbar2.strength = hnsswstrength;
            m_swinglowbar2.length = mHnsswlength;

            m_swinghighbar1.instance = 1;
            m_swinghighbar1.price = Bars.High;
            m_swinghighbar1.strength = hnsswstrength;
            m_swinghighbar1.length = mHnsswlength;

            m_swinghighbar2.instance = 1;
            m_swinghighbar2.price = Bars.High;
            m_swinghighbar2.strength = neckswstrength;
            m_swinghighbar2.length = mNeckswlength;

            m_color.DefaultValue = color2;
            m_tlref = null;
        }


        protected override void CalcBar(){
            m_barnum.Value = m_barnumber1[0];
            if ((hilo == 1)){
                m_neckswbar.Value = m_swinglowbar1[0];
                m_hnsswbar.Value = m_swinghighbar1[0];
                m_neckswprice.Value = Bars.Low[0];
                m_hnsswprice.Value = Bars.High[0];
            }
            else{
                if ((hilo
                     == (-1))){
                    m_neckswbar.Value = m_swinghighbar2[0];
                    m_hnsswbar.Value = m_swinglowbar2[0];
                    m_neckswprice.Value = Bars.High[0];
                    m_hnsswprice.Value = Bars.Low[0];
                }
            }
            if ((m_neckswbar.Value == neckswstrength)){
                m_neckswings[0][((1)), 0] = m_neckswings[0][0, 0];
                m_neckswings[0][((1)), ((1))] = m_neckswings[0][0, ((1))];
                m_neckswings[0][((1)), ((2))] = m_neckswings[0][0, ((2))];
                m_neckswings[0][((1)), ((3))] = m_neckswings[0][0, ((3))];
                m_neckswings[0][0, 0] = m_barnum[neckswstrength];
                m_neckswings[0][0, ((1))] = Bars.Time[neckswstrength].ToOADate();
                m_neckswings[0][0, ((2))] = Bars.Time[neckswstrength].ToOADate();
                m_neckswings[0][0, ((3))] = m_neckswprice[neckswstrength];
            }
            if ((m_hnsswbar.Value == hnsswstrength)){
                for (var m_value1 = 1; PublicFunctions.DoubleGreaterEquals(m_value1, 0); m_value1 = (m_value1 - 1)){
                    m_hnsswings[0][m_value1 + 1, 0] = m_hnsswings[0][m_value1, 0];
                    m_hnsswings[0][m_value1 + 1, ((1))] = m_hnsswings[0][m_value1, ((1))];
                    m_hnsswings[0][m_value1 + 1, ((2))] = m_hnsswings[0][m_value1, ((2))];
                    m_hnsswings[0][m_value1 + 1, ((3))] = m_hnsswings[0][m_value1, ((3))];
                    m_hnsswings[0][m_value1 + 1, ((4))] = m_hnsswings[0][m_value1, ((4))];
                }
                m_hnsswings[0][0, 0] = m_barnum[hnsswstrength];
                m_hnsswings[0][0, ((1))] = Bars.Time[hnsswstrength].ToOADate();
                m_hnsswings[0][0, ((2))] = Bars.Time[hnsswstrength].ToOADate();
                m_hnsswings[0][0, ((3))] = m_hnsswprice[hnsswstrength];
                m_hnsswings[0][0, ((4))] = Bars.TrueVolume().Average(hnsswstrength*2);

                var m_hororder = ((((m_hnsswings[0][0, 0] > m_neckswings[0][0, 0])
                                && (m_neckswings[0][0, 0] > m_hnsswings[0][((1)), 0]))
                               && (m_hnsswings[0][((1)), 0] > m_neckswings[0][((1)), 0]))
                              && (m_neckswings[0][((1)), 0] > m_hnsswings[0][((2)), 0]));
                if ((hilo == 1)){
                    m_vertorder_hvss.Value = ((m_hnsswings[0][((1)), ((3))] > m_hnsswings[0][((2)), ((3))])
                                              && (m_hnsswings[0][((1)), ((3))] > m_hnsswings[0][0, ((3))]));
                    m_vertorder_nvss.Value = ((m_neckswings[0][0, ((3))] < m_hnsswings[0][((2)), ((3))])
                                              && (m_neckswings[0][((1)), ((3))] < m_hnsswings[0][0, ((3))]));
                    m_necklinelimit.Value = PublicFunctions.Min(m_neckswings[0][0, ((3))],
                                                                m_neckswings[0][((1)), ((3))]);
                    m_maxnecktoheaddist.Value = (m_hnsswings[0][((1)), ((3))] - m_necklinelimit.Value);
                }
                else{
                    if ((hilo == (-1))){
                        m_vertorder_hvss.Value = ((m_hnsswings[0][((1)), ((3))] < m_hnsswings[0][((2)), ((3))])
                                                  && (m_hnsswings[0][((1)), ((3))] < m_hnsswings[0][0, ((3))]));
                        m_vertorder_nvss.Value = ((m_neckswings[0][0, ((3))] > m_hnsswings[0][((2)), ((3))])
                                                  && (m_neckswings[0][((1)), ((3))] > m_hnsswings[0][0, ((3))]));
                        m_necklinelimit.Value = PublicFunctions.Max(m_neckswings[0][0, ((3))],
                                                                    m_neckswings[0][((1)), ((3))]);
                        m_maxnecktoheaddist.Value = (m_necklinelimit.Value - m_hnsswings[0][((1)), ((3))]);
                    }
                }
                var m_necklinetilt = Math.Abs((m_neckswings[0][0, ((3))] - m_neckswings[0][((1)), ((3))]));
                if ((m_maxnecktoheaddist.Value != 0)){
                    m_vertproportion.Value = PublicFunctions.DoubleLess((m_necklinetilt/((m_maxnecktoheaddist.Value))),
                                                                        vertproportionlimit);
                }
                else{
                    m_vertproportion.Value = false;
                }
                if ((((m_hororder && m_vertorder_hvss.Value)
                      && m_vertorder_nvss.Value)
                     && m_vertproportion.Value)){
                    if ((m_color.Value == color2)){
                        m_color.Value = color1;
                    }
                    else{
                        m_color.Value = color2;
                    }
                    var m_textref =
                        DrwText.Create(
                            new ChartPoint(DateTime.FromOADate(m_hnsswings[0][0, ((2))]),
                                           m_hnsswings[0][0, ((3))]), "Shoulder");
                    m_textref.Color = m_color.Value;

                    m_textref.HStyle = (ETextStyleH) 2;
                    m_textref.VStyle = 0;

                    m_textref =
                        DrwText.Create(
                            new ChartPoint(DateTime.FromOADate(m_hnsswings[0][((1)), ((2))]),
                                           m_hnsswings[0][((1)), ((3))]), "Head");
                    m_textref.Color = m_color.Value;

                    m_textref.HStyle = (ETextStyleH) 2;
                    m_textref.VStyle = 0;

                    m_textref =
                        DrwText.Create(
                            new ChartPoint(DateTime.FromOADate(m_hnsswings[0][((2)), ((2))]),
                                           m_hnsswings[0][((2)), ((3))]), "Shoulder");
                    m_textref.Color = m_color.Value;

                    m_textref.HStyle = (ETextStyleH) 2;
                    m_textref.VStyle = 0;

                    if (confirmvolume)
                    {
                        var m_avgvolfast = Bars.TrueVolume().Average(hnsswstrength);
                        string m_msg;
                        if ((PublicFunctions.DoubleGreater(m_avgvolfast, m_hnsswings[0][0, ((4))]) &&
                             PublicFunctions.DoubleGreater(m_avgvolfast, m_hnsswings[0][((2)), ((4))]))){
                            m_msg = "Volume confirm";
                        }
                        else{
                            m_msg = "Vol non-confirm";
                        }
                        m_textref =
                            DrwText.Create(
                                new ChartPoint(DateTime.FromOADate(m_neckswings[0][((1)), ((2))]),
                                               m_necklinelimit.Value), m_msg);
                        m_textref.Color = m_color.Value;

                        m_textref.HStyle = 0;
                        m_textref.VStyle = (ETextStyleV) 1;
                    }
                    if (drawneckline){
                        m_tlref =
                            DrwTrendLine.Create(
                                new ChartPoint(DateTime.FromOADate(m_neckswings[0][((1)), ((2))]),
                                               m_neckswings[0][((1)), ((3))]),
                                new ChartPoint(DateTime.FromOADate(m_neckswings[0][0, ((2))]),
                                               m_neckswings[0][0, ((3))]));
                        m_tlref.ExtLeft = false;
                        m_tlref.ExtRight = false;
                        m_tlref.Color = m_color.Value;
                        m_confirmbarnum.Value = m_barnum.Value;
                    }
                }
            }
            if ((null != m_tlref && m_tlref.Exist && m_lastcrossedtl.Value != m_tlref.ID
                 && PublicFunctions.DoubleLessEquals((m_barnum.Value - m_confirmbarnum.Value), barspast))){
                var m_tl_value = m_tlref.PriceValue(Bars.Time[0]);
                if ((((hilo == 1)
                      && PublicFunctions.DoubleLess(Bars.Close[0], m_tl_value))
                     || ((hilo
                          == (-1))
                         && PublicFunctions.DoubleGreater(Bars.Close[0], m_tl_value)))){
                    Plot1.Set(0, m_neckswprice.Value, m_color.Value);
                    Alerts.Alert();
                    m_lastcrossedtl.Value = m_tlref.ID;
                }
                else{
                    Plot1.Reset();
                }
            }
        }
    }
}