using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Crossunders_per_Bar : IndicatorObject
    {
        private VariableObject<Int32> m_mycurrentbar;
        private VariableObject<Double> m_crossunders;
        private VariableObject<Boolean> m_lastabove;

        private IPlotObject Plot1;

        public Crossunders_per_Bar(object ctx) :
            base(ctx){
            datanum = 1;
        }

        private ISeries<double> formula1 { get; set; }
        private ISeries<double> formula2 { get; set; }

        [Input]
        public double datanum { get; set; }

        protected override void Create(){
            m_mycurrentbar = new VariableObject<Int32>(this);
            m_crossunders = new VariableObject<Double>(this);
            m_lastabove = new VariableObject<Boolean>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Crossunders", EPlotShapes.Histogram,
                                           Color.Magenta, Color.Empty, 0,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            formula1 = Bars.Close;
            formula2 = new Lambda<double>(_bb => Bars.Close.Average(9, _bb));
        }


        protected override void CalcBar(){
            var m_formula2 = formula2.Value;
            if ((Bars.LastBarOnChart && (Bars.Status != EBarState.Close))){
                if (Bars.CurrentBar > m_mycurrentbar.Value){
                    m_crossunders.Value = 0;
                    m_mycurrentbar.Value = Bars.CurrentBar;
                }
                if ((PublicFunctions.DoubleLess(formula1[0], m_formula2) && m_lastabove.Value)){
                    m_crossunders.Value = (m_crossunders.Value + 1);
                    m_lastabove.Value = false;
                }
                else{
                    if (PublicFunctions.DoubleGreater(formula1[0], m_formula2)){
                        m_lastabove.Value = true;
                    }
                }
                Plot1.Set(0, m_crossunders.Value);
            }
        }
    }
}