using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Pivot_Rev_Gen__Dn : IndicatorObject
    {
        private PivotReversalGen m_pivotreversalgen1;

        private VariableObject<Double> m_opivotprice;
        private VariableObject<Double> m_opivotrstren;
        private VariableObject<Double> m_opivotlstren;

        private IPlotObject Plot1;

        public Pivot_Rev_Gen__Dn(object ctx) :
            base(ctx){
            plotcolor = Color.Cyan;
            linescolor = Color.Magenta;
            colorscheme = 1;
            drawlines = true;
            lrfactor = 1;
            maxrstren = 30;
            minrstren = 1;
        }

        [Input]
        public double minrstren { get; set; }

        [Input]
        public double maxrstren { get; set; }

        [Input]
        public double lrfactor { get; set; }

        [Input]
        public bool drawlines { get; set; }

        [Input]
        public Color linescolor { get; set; }

        [Input]
        public Color plotcolor { get; set; }

        [Input]
        public int colorscheme { get; set; }

        protected override void Create(){
            m_pivotreversalgen1 = new PivotReversalGen(this);
            m_opivotprice = new VariableObject<Double>(this);
            m_opivotrstren = new VariableObject<Double>(this);
            m_opivotlstren = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("PivRev_Dn", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_pivotreversalgen1.minrstren = minrstren;
            m_pivotreversalgen1.maxrstren = maxrstren;
            m_pivotreversalgen1.lrfactor = lrfactor;
            m_pivotreversalgen1.hilo = -1;
            m_pivotreversalgen1.drawlines = drawlines;
            m_pivotreversalgen1.linescolor = linescolor;
            m_pivotreversalgen1.opivotprice = m_opivotprice;
            m_pivotreversalgen1.opivotrstren = m_opivotrstren;
            m_pivotreversalgen1.opivotlstren = m_opivotlstren;
        }


        protected override void CalcBar(){
            if (m_pivotreversalgen1[0] == 1){
                Plot1.Set(0, m_opivotprice.Value);
                if ((colorscheme == 1)){
                    Plot1.Colors[0] = plotcolor;
                }
                else{
                    if ((colorscheme == 2)){
                        Plot1.BGColor = plotcolor;
                    }
                }
                Alerts.Alert(((((("Pivot reversal: Lo = " + Math.Round(m_opivotprice.Value, 2))
                                 + ", RS = ")
                                + Math.Round(m_opivotrstren.Value, 0))
                               + ", LS = ")
                              + Math.Round(m_opivotlstren.Value, 0)));
            }
        }
    }
}