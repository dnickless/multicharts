using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Formula_OHLC : IndicatorObject
    {
        private Function.RSI m_rsi1;

        private VariableObject<int> m_barnum;

        private VariableObject<Double> m_formulaopen;

        private VariableObject<Double> m_formulahigh;

        private VariableObject<Double> m_formulalow;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        private IPlotObject Plot5;

        private IPlotObject Plot6;

        public Formula_OHLC(object ctx) :
            base(ctx){
            plotbaseline2 = true;
            plotbaseline1 = true;
            baseline2 = 30;
            baseline1 = 70;
        }

        [Input]
        public double baseline1 { get; set; }

        [Input]
        public double baseline2 { get; set; }

        [Input]
        public bool plotbaseline1 { get; set; }

        [Input]
        public bool plotbaseline2 { get; set; }

        protected override void Create(){
            m_rsi1 = new Function.RSI(this);
            m_barnum = new VariableObject<int>(this);
            m_formulaopen = new VariableObject<Double>(this);
            m_formulahigh = new VariableObject<Double>(this);
            m_formulalow = new VariableObject<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("FormulaOpen", EPlotShapes.LeftTick,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("FormulaHigh", EPlotShapes.BarHigh,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("FormulaLow", EPlotShapes.BarLow,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("FormulaClose", EPlotShapes.RightTick,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
            Plot5 =
                AddPlot(new PlotAttributes("Baseline1", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot6 =
                AddPlot(new PlotAttributes("Baseline2", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_rsi1.price = Bars.Close;
            m_rsi1.length = 14;
        }


        protected override void CalcBar(){
            var m_formula = m_rsi1[0];
            if (Environment.IsRealTimeCalc){
                if ((m_barnum.Value != Bars.CurrentBar)){
                    m_formulaopen.Value = m_formula;
                    m_formulahigh.Value = m_formula;
                    m_formulalow.Value = m_formula;
                    m_barnum.Value = Bars.CurrentBar;
                }
                if (PublicFunctions.DoubleGreater(m_formula, m_formulahigh.Value)){
                    m_formulahigh.Value = m_formula;
                }
                if (PublicFunctions.DoubleLess(m_formula, m_formulalow.Value)){
                    m_formulalow.Value = m_formula;
                }
                Plot1.Set(0, m_formulaopen.Value);
                Plot2.Set(0, m_formulahigh.Value);
                Plot3.Set(0, m_formulalow.Value);
                Plot4.Set(0, m_formula);
            }
            if (plotbaseline1){
                Plot5.Set(0, baseline1);
            }
            if (plotbaseline2){
                Plot6.Set(0, baseline2);
            }
        }
    }
}