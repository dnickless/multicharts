using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class Crossovers_per_Bar : IndicatorObject
    {
        private VariableObject<Int32> m_mycurrentbar;
        private VariableObject<Double> m_crossovers;
        private VariableObject<Boolean> m_lastbelow;

        private IPlotObject Plot1;

        public Crossovers_per_Bar(object ctx) :
            base(ctx){
            datanum = 1;
        }

        private ISeries<double> formula1 { get; set; }

        [Input]
        public double datanum { get; set; }

        protected override void Create(){
            m_mycurrentbar = new VariableObject<Int32>(this);
            m_crossovers = new VariableObject<Double>(this);
            m_lastbelow = new VariableObject<Boolean>(this);
            Plot1 =
                AddPlot(new PlotAttributes("Crossovers", EPlotShapes.Histogram,
                                           Color.Cyan, Color.Empty, 0, 0,
                                           true));
        }

        protected override void StartCalc(){
            formula1 = Bars.Close;
        }


        protected override void CalcBar(){
            var m_formula2 = Bars.Close.Average(9);
            if ((Bars.LastBarOnChart && (Bars.Status != EBarState.Close))){
                if (Bars.CurrentBar > m_mycurrentbar.Value){
                    m_crossovers.Value = 0;
                    m_mycurrentbar.Value = Bars.CurrentBar;
                }
                if ((PublicFunctions.DoubleGreater(formula1[0], m_formula2) && m_lastbelow.Value)){
                    m_crossovers.Value = (m_crossovers.Value + 1);
                    m_lastbelow.Value = false;
                }
                else{
                    if (PublicFunctions.DoubleLess(formula1[0], m_formula2)){
                        m_lastbelow.Value = true;
                    }
                }
                Plot1.Set(0, m_crossovers.Value);
            }
        }
    }
}