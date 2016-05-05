using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Linear_Reg_Line : IndicatorObject
    {
        private LinearRegValue m_linearregvalue1;

        private LinearRegValue m_linearregvalue2;

        private LinearRegValue m_linearregvalue3;

        private LinearRegValue m_linearregvalue4;

        private LinearRegValue m_linearregvalue5;

        private LinearRegValue m_linearregvalue6;

        private VariableObject<ITrendLineObject> m_tllrv;

        private VariableObject<int> m_flag;

        public Linear_Reg_Line(object ctx) :
            base(ctx){
            extright = true;
            color = Color.Yellow;
            endtime = DateTime.MinValue;
            length = 30;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public DateTime endtime { get; set; }

        [Input]
        public Color color { get; set; }

        [Input]
        public bool extright { get; set; }

        protected override void Create(){
            m_linearregvalue1 = new LinearRegValue(this);
            m_linearregvalue2 = new LinearRegValue(this);
            m_linearregvalue3 = new LinearRegValue(this);
            m_linearregvalue4 = new LinearRegValue(this);
            m_linearregvalue5 = new LinearRegValue(this);
            m_linearregvalue6 = new LinearRegValue(this);
            m_tllrv = new VariableObject<ITrendLineObject>(this);
            m_flag = new VariableObject<int>(this);
        }

        protected override void StartCalc(){
            m_linearregvalue1.price = Bars.Close;
            m_linearregvalue1.length = length;
            m_linearregvalue1.tgtbar = 0;
            m_linearregvalue2.price = Bars.Close;
            m_linearregvalue2.length = length;
            m_linearregvalue2.tgtbar = length - 1;
            m_linearregvalue3.price = Bars.Close;
            m_linearregvalue3.length = length;
            m_linearregvalue3.tgtbar = 0;
            m_linearregvalue4.price = Bars.Close;
            m_linearregvalue4.length = length;
            m_linearregvalue4.tgtbar = length - 1;
            m_linearregvalue5.price = Bars.Close;
            m_linearregvalue5.length = length;
            m_linearregvalue5.tgtbar = 0;
            m_linearregvalue6.price = Bars.Close;
            m_linearregvalue6.length = length;
            m_linearregvalue6.tgtbar = length;
            m_tllrv.DefaultValue = null;
            m_flag.DefaultValue = 0;
        }


        protected override void CalcBar(){
            if ((m_flag.Value == 0)){
                if (((endtime == DateTime.MinValue)
                     && Bars.LastBarOnChart)){
                    var m_lrv = m_linearregvalue1[0];
                    var m_lrvago = m_linearregvalue2[0];
                    m_tllrv.Value = DrwTrendLine.Create(new ChartPoint(Bars.Time[(length - 1)], m_lrvago),
                                                        new ChartPoint(Bars.Time[0], m_lrv));
                    m_flag.Value = 1;
                }
                else{
                    if (Bars.Time[0] == endtime)
                    {
                        var m_lrv = m_linearregvalue3[0];
                        var m_lrvago = m_linearregvalue4[0];
                        m_tllrv.Value = DrwTrendLine.Create(new ChartPoint(Bars.Time[(length - 1)], m_lrvago),
                                                            new ChartPoint(Bars.Time[0], m_lrv));
                        m_flag.Value = 2;
                    }
                }
                if (((m_flag.Value == 1)
                     || (m_flag.Value == 2))){
                    m_tllrv.Value.Color = color;
                    m_tllrv.Value.ExtLeft = false;
                    if (extright){
                        m_tllrv.Value.ExtRight = true;
                    }
                    else{
                        m_tllrv.Value.ExtRight = false;
                    }
                }
            }
            else{
                if ((m_flag.Value == 1)){
                    var m_lrv = m_linearregvalue5[0];
                    var m_lrvago = m_linearregvalue6[0];
                    m_tllrv.Value.Begin = new ChartPoint(Bars.Time[(length - 1)], m_lrvago);
                    m_tllrv.Value.End = new ChartPoint(Bars.Time[0], m_lrv);
                }
            }
        }
    }
}