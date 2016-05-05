using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Pennant_Brkout : IndicatorObject
    {
        private Pennant m_pennant1;

        private VariableObject<Double> m_otlhistartpr;

        private VariableObject<Double> m_otlhiendpr;

        private VariableObject<Double> m_otllostartpr;

        private VariableObject<Double> m_otlloendpr;

        private VariableObject<Color> m_color;

        private VariableObject<ITrendLineObject> m_tlhi;

        private VariableObject<ITrendLineObject> m_tllo;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Pennant_Brkout(object ctx) :
            base(ctx){
            color2 = Color.Magenta;
            color1 = Color.Red;
            drawextensions = true;
            drawlines = true;
            barspast = 5;
            maxconsolindex = 1.5;
            length = 7;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double maxconsolindex { get; set; }

        [Input]
        public int barspast { get; set; }

        [Input]
        public bool drawlines { get; set; }

        [Input]
        public bool drawextensions { get; set; }

        [Input]
        public Color color1 { get; set; }

        [Input]
        public Color color2 { get; set; }

        protected override void Create(){
            m_pennant1 = new Pennant(this);
            m_otlhistartpr = new VariableObject<Double>(this);
            m_otlhiendpr = new VariableObject<Double>(this);
            m_otllostartpr = new VariableObject<Double>(this);
            m_otlloendpr = new VariableObject<Double>(this);
            m_color = new VariableObject<Color>(this);
            m_tlhi = new VariableObject<ITrendLineObject>(this);
            m_tllo = new VariableObject<ITrendLineObject>(this);
            Plot1 =
                AddPlot(new PlotAttributes("PenBrkoutUp", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 5, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("PenBrkoutDn", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 5,
                                           0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("HiExt", EPlotShapes.Point,
                                           Color.Silver, Color.Empty, 3,
                                           0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("LoExt", EPlotShapes.Point,
                                           Color.Silver, Color.Empty, 3,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_pennant1.length = length;
            m_pennant1.maxconsolindex = maxconsolindex;
            m_pennant1.barspast = barspast;
            m_pennant1.otlhistartpr = m_otlhistartpr;
            m_pennant1.otlhiendpr = m_otlhiendpr;
            m_pennant1.otllostartpr = m_otllostartpr;
            m_pennant1.otlloendpr = m_otlloendpr;
            m_color.DefaultValue = color2;
        }


        protected override void CalcBar(){
            var m_value1 = m_pennant1[0];
            if ((m_value1 == 1))
            {
                m_color.Value = (m_color.Value == color2) ? color1 : color2;
                if (drawlines){
                    var mLengthminus1 = (length - 1);
                    m_tlhi.Value =
                        DrwTrendLine.Create(new ChartPoint(Bars.Time[mLengthminus1], m_otlhistartpr.Value),
                                            new ChartPoint(Bars.Time[0], m_otlhiendpr.Value));
                    m_tlhi.Value.ExtLeft = false;
                    m_tlhi.Value.ExtRight = false;
                    m_tlhi.Value.Color = m_color.Value;
                    m_tllo.Value =
                        DrwTrendLine.Create(new ChartPoint(Bars.Time[mLengthminus1], m_otllostartpr.Value),
                                            new ChartPoint(Bars.Time[0], m_otlloendpr.Value));
                    m_tllo.Value.ExtLeft = false;
                    m_tllo.Value.ExtRight = false;
                    m_tllo.Value.Color = m_color.Value;
                }
            }
            else{
                if (((m_value1 == 2)
                     || (m_value1 == 3))){
                    if (drawlines){
                        m_tlhi.Value.Size = ((2));
                        m_tllo.Value.Size = ((2));
                    }
                    if ((m_value1 == 2)){
                        Plot1.Set(0, Bars.High[0]);
                    }
                    else{
                        if ((m_value1 == 3)){
                            Plot2.Set(0, Bars.Low[0]);
                        }
                    }
                }
            }
            if (drawextensions){
                if (PublicFunctions.DoubleGreater(m_otlhiendpr.Value, 0)){
                    Plot3.Set(0, m_otlhiendpr.Value, m_color.Value);
                }
                if (PublicFunctions.DoubleGreater(m_otlloendpr.Value, 0)){
                    Plot4.Set(0, m_otlloendpr.Value, m_color.Value);
                }
            }
        }
    }
}