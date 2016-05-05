using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class ZigZag_Pnts : IndicatorObject
    {
        private SwingHigh m_swinghigh1;

        private SwingLow m_swinglow1;

        private VariableSeries<Double> m_swingprice;

        private VariableSeries<DateTime> m_swingtime;

        private VariableObject<int> m_tldir;

        private VariableObject<Boolean> m_saveswing;

        private VariableObject<Boolean> m_addtl;

        private VariableObject<Boolean> m_updatetl;

        private VariableObject<ITrendLineObject> m_tlref;

        public ZigZag_Pnts(object ctx) :
            base(ctx){
            linewidth = 1;
            linecolor = Color.Yellow;
            retracepnts = 5;
        }

        private ISeries<double> price { get; set; }

        [Input]
        public double retracepnts { get; set; }

        [Input]
        public Color linecolor { get; set; }

        [Input]
        public double linewidth { get; set; }

        protected override void Create(){
            m_swinghigh1 = new SwingHigh(this);
            m_swinglow1 = new SwingLow(this);
            m_swingprice = new VariableSeries<Double>(this);
            m_swingtime = new VariableSeries<DateTime>(this);
            m_tldir = new VariableObject<int>(this);
            m_saveswing = new VariableObject<Boolean>(this);
            m_addtl = new VariableObject<Boolean>(this);
            m_updatetl = new VariableObject<Boolean>(this);
            m_tlref = new VariableObject<ITrendLineObject>(this);
        }

        protected override void StartCalc(){
            price = Bars.Close;
            m_swinghigh1.instance = 1;
            m_swinghigh1.price = price;
            m_swinghigh1.strength = 1;
            m_swinghigh1.length = 2;
            m_swinglow1.instance = 1;
            m_swinglow1.price = price;
            m_swinglow1.strength = 1;
            m_swinglow1.length = 2;
            m_swingprice.DefaultValue = price[0];
            m_swingtime.DefaultValue = Bars.Time[0];
        }


        protected override void CalcBar(){
            var m_newswingprice = m_swinghigh1[0];
            if ((m_newswingprice != (-1))){
                if ((PublicFunctions.DoubleLessEquals(m_tldir.Value, 0) &&
                     PublicFunctions.DoubleGreaterEquals(m_newswingprice, (m_swingprice.Value + retracepnts)))){
                    m_saveswing.Value = true;
                    m_addtl.Value = true;
                    m_tldir.Value = 1;
                }
                else{
                    if (((m_tldir.Value == 1)
                         && PublicFunctions.DoubleGreaterEquals(m_newswingprice, m_swingprice.Value))){
                        m_saveswing.Value = true;
                        m_updatetl.Value = true;
                    }
                }
            }
            else{
                m_newswingprice = m_swinglow1[0];
                if ((m_newswingprice != (-1))){
                    if ((PublicFunctions.DoubleGreaterEquals(m_tldir.Value, 0) &&
                         PublicFunctions.DoubleLessEquals(m_newswingprice, (m_swingprice.Value - retracepnts)))){
                        m_saveswing.Value = true;
                        m_addtl.Value = true;
                        m_tldir.Value = (-1);
                    }
                    else{
                        if (((m_tldir.Value
                              == (-1))
                             && PublicFunctions.DoubleLessEquals(m_newswingprice, m_swingprice.Value))){
                            m_saveswing.Value = true;
                            m_updatetl.Value = true;
                        }
                    }
                }
            }
            if (m_saveswing.Value){
                m_swingprice.Value = m_newswingprice;
                m_swingtime.Value = Bars.Time[1];
                m_saveswing.Value = false;
            }
            if (m_addtl.Value){
                m_tlref.Value = DrwTrendLine.Create(new ChartPoint(m_swingtime.Value, m_swingprice.Value),
                                                    new ChartPoint(m_swingtime[1], m_swingprice[1]));
                m_tlref.Value.ExtLeft = false;
                m_tlref.Value.ExtRight = false;
                m_tlref.Value.Size = ((int) (linewidth));
                m_tlref.Value.Color = linecolor;
                m_addtl.Value = false;
            }
            else{
                if (m_updatetl.Value){
                    m_tlref.Value.End = new ChartPoint(m_swingtime.Value, m_swingprice.Value);
                    m_updatetl.Value = false;
                }
            }
        }
    }
}