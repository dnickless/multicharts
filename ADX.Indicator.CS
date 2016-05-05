using System;
using System.Drawing;

namespace PowerLanguage.Indicator
{
    public class ADX : IndicatorObject
    {
        private Function.ADX m_adx1;

        private VariableSeries<Double> m_adxvalue;

        private IPlotObject Plot1;

        public ADX(object ctx) :
            base(ctx){
            length = 14;
        }

        [Input]
        public int length { get; set; }

        protected override void Create(){
            m_adx1 = new Function.ADX(this);
            m_adxvalue = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("ADX", 0, Color.Cyan,
                                            Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_adx1.Length = length;
            m_adxvalue.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_adxvalue.Value = m_adx1[0];
            Plot1.Set(0, m_adxvalue.Value);
            if (PublicFunctions.DoubleGreater(m_adxvalue.Value, m_adxvalue[1]) 
                &&PublicFunctions.DoubleLessEquals(m_adxvalue[1], m_adxvalue[2])){
                Alerts.Alert("Indicator turning up");
            }
            else{
                if (PublicFunctions.DoubleLess(m_adxvalue.Value, m_adxvalue[1]) 
                    &&PublicFunctions.DoubleGreaterEquals(m_adxvalue[1], m_adxvalue[2])){
                    Alerts.Alert("Indicator turning down");
                }
            }
        }
    }
}