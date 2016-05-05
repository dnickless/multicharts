using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_ShootingStar : IndicatorObject
    {
        private Function.C_ShootingStar m_c_shootingstar1;

        private IPlotObject Plot1;

        public C_ShootingStar(object ctx) :
            base(ctx){
            factor = 2;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double factor { get; set; }

        protected override void Create(){
            m_c_shootingstar1 = new Function.C_ShootingStar(this);
            Plot1 =
                AddPlot(new PlotAttributes("ShStar", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_shootingstar1.length = length;
            m_c_shootingstar1.factor = factor;
        }


        protected override void CalcBar(){
            if (m_c_shootingstar1[0] == 1){
                Plot1.Set(0, Bars.Close[0]);
                Alerts.Alert("ShootingStar");
            }
        }
    }
}