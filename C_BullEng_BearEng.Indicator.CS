using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class C_BullEng_BearEng : IndicatorObject
    {
        private int m_length = 14;

        private Function.C_BullEng_BearEng m_c_bulleng_beareng1;

        private VariableObject<int> m_obullishengulfing;

        private VariableObject<int> m_obearishengulfing;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        public C_BullEng_BearEng(object ctx) :
            base(ctx) {}

        [Input]
        public int length{
            get { return m_length; }
            set { m_length = value; }
        }

        protected override void Create(){
            m_c_bulleng_beareng1 = new Function.C_BullEng_BearEng(this);
            m_obullishengulfing = new VariableObject<int>(this);
            m_obearishengulfing = new VariableObject<int>(this);
            Plot1 =
                AddPlot(new PlotAttributes("BullEng", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("BearEng", EPlotShapes.Point,
                                           Color.Magenta, Color.Empty, 4,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_c_bulleng_beareng1.Length = length;
            m_c_bulleng_beareng1.obullishengulfing = m_obullishengulfing;
            m_c_bulleng_beareng1.obearishengulfing = m_obearishengulfing;
            m_obullishengulfing.DefaultValue = 0;
            m_obearishengulfing.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_c_bulleng_beareng1.Call();
            if (m_obullishengulfing.Value == 1){
                Plot1.Set(0, Bars.High[0]);
                Alerts.Alert("BullishEngulfing");
            }
            else{
                if (m_obearishengulfing.Value == 1){
                    Plot2.Set(0, Bars.Low[0]);
                    Alerts.Alert("BearishEngulfing");
                }
            }
        }
    }
}