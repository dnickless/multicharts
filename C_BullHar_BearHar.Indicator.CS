using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_BullHar_BearHar : IndicatorObject
    {
        private Function.C_BullHar_BearHar m_c_bullhar_bearhar1;

        private VariableObject<int> m_obullishharami;

        private VariableObject<int> m_obearishharami;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_BullHar_BearHar(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_c_bullhar_bearhar1 = new Function.C_BullHar_BearHar(this);
            m_obullishharami = new VariableObject<int>(this);
            m_obearishharami = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("BullHar", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("BearHar", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_bullhar_bearhar1.length = length;
            m_c_bullhar_bearhar1.obullishharami = m_obullishharami;
            m_c_bullhar_bearhar1.obearishharami = m_obearishharami;
            m_obullishharami.DefaultValue = 0;
            m_obearishharami.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_bullhar_bearhar1.Call();
            if ((m_obullishharami.Value == 1)){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("BullishHarami");
            }
            else{
                if ((m_obearishharami.Value == 1)){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("BearishHarami");
                }
            }
        }
    }
}