using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_MornDoji_EveDoji : IndicatorObject
    {
        private Function.C_MornDoji_EveDoji m_c_morndoji_evedoji1;

        private VariableObject<int> m_omorningdojistar;

        private VariableObject<int> m_oeveningdojistar;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_MornDoji_EveDoji(object ctx) :
            base(ctx){
            percent = 5;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double percent { get; set; }

        protected override void Create(){
            m_c_morndoji_evedoji1 = new Function.C_MornDoji_EveDoji(this);
            m_omorningdojistar = new VariableObject<int>(this);
            m_oeveningdojistar = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MornDoji", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("EveDoji", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_morndoji_evedoji1.length = length;
            m_c_morndoji_evedoji1.percent = percent;
            m_c_morndoji_evedoji1.omorningdojistar = m_omorningdojistar;
            m_c_morndoji_evedoji1.oeveningdojistar = m_oeveningdojistar;
            
            m_omorningdojistar.DefaultValue = 0;
            m_oeveningdojistar.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_morndoji_evedoji1.Call();
            if (m_omorningdojistar.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("MorningDojiStar");
            }
            else{
                if (m_oeveningdojistar.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("EveningDojiStar");
                }
            }
        }
    }
}