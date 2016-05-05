using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_MornStar_EveStar : IndicatorObject
    {
        private Function.C_MornStar_EveStar m_c_mornstar_evestar1;

        private VariableObject<int> m_omorningstar;

        private VariableObject<int> m_oeveningstar;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_MornStar_EveStar(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_c_mornstar_evestar1 = new Function.C_MornStar_EveStar(this);
            m_omorningstar = new VariableObject<int>(this);
            m_oeveningstar = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MornStar", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("EveStar", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 4, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_mornstar_evestar1.length = length;
            m_c_mornstar_evestar1.omorningstar = m_omorningstar;
            m_c_mornstar_evestar1.oeveningstar = m_oeveningstar;
            m_omorningstar.DefaultValue = 0;
            m_oeveningstar.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_mornstar_evestar1.Call();
            if (m_omorningstar.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("MorningStar");
            }
            else{
                if (m_oeveningstar.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("EveningStar");
                }
            }
        }
    }
}