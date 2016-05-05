using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class ZigZag_Pcnt : IndicatorObject
    {
        private ISeries<Double> m_price;

        private double m_retracepct = 5;

        private Color m_linecolor = Color.Yellow;

        private double m_linewidth = 1;

        private SwingHigh m_swinghigh1;

        private SwingLow m_swinglow1;

        private double m_newswingprice;

        private VariableSeries<Double> m_swingprice;

        private VariableSeries<DateTime> m_swingtime;

        private VariableObject<Double> m_tldir;

        private double m_retracefctrup;

        private double m_retracefctrdn;

        private VariableObject<Boolean> m_saveswing;

        private VariableObject<Boolean> m_addtl;

        private VariableObject<Boolean> m_updatetl;

        private VariableObject<ITrendLineObject> m_tlref;

        public ZigZag_Pcnt(object ctx) :
            base(ctx) {}

        private ISeries<Double> price{
            get { return m_price; }
        }

        [Input]
        public double retracepct{
            get { return m_retracepct; }
            set { m_retracepct = value; }
        }

        [Input]
        public KnownColor linecolor{
            get { return m_linecolor.ToKnownColor(); }
            set { m_linecolor = Color.FromKnownColor(value); }
        }

        [Input]
        public double linewidth{
            get { return m_linewidth; }
            set { m_linewidth = value; }
        }

        protected override void Create(){
            m_swinghigh1 = new SwingHigh(this);
            m_swinglow1 = new SwingLow(this);
            m_swingprice = new VariableSeries<Double>(this);
            m_swingtime = new VariableSeries<DateTime>(this);
            m_tldir = new VariableObject<Double>(this);
            m_saveswing = new VariableObject<Boolean>(this);
            m_addtl = new VariableObject<Boolean>(this);
            m_updatetl = new VariableObject<Boolean>(this);
            m_tlref = new VariableObject<ITrendLineObject>(this);
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
            m_swinghigh1.instance = 1;
            m_swinghigh1.price = price;
            m_swinghigh1.strength = 1;
            m_swinghigh1.length = 2;
            m_swinglow1.instance = 1;
            m_swinglow1.price = price;
            m_swinglow1.strength = 1;
            m_swinglow1.length = 2;
            m_newswingprice = 0;
            m_swingprice.DefaultValue = price[0];
            m_swingtime.DefaultValue = Bars.Time[0];
            m_tldir.DefaultValue = 0;
            m_retracefctrup = (1
                               + (retracepct*0.01));
            m_retracefctrdn = (1
                               - (retracepct*0.01));
            m_saveswing.DefaultValue = false;
            m_addtl.DefaultValue = false;
            m_updatetl.DefaultValue = false;
            m_tlref.DefaultValue = null;
        }


        protected override void CalcBar(){
            m_newswingprice = m_swinghigh1[0];
            if ((m_newswingprice
                 != (-1))){
                if ((PublicFunctions.DoubleLessEquals(m_tldir.Value, 0) &&
                     PublicFunctions.DoubleGreaterEquals(m_newswingprice, (m_swingprice.Value*m_retracefctrup)))){
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
                if ((m_newswingprice
                     != (-1))){
                    if ((PublicFunctions.DoubleGreaterEquals(m_tldir.Value, 0) &&
                         PublicFunctions.DoubleLessEquals(m_newswingprice, (m_swingprice.Value*m_retracefctrdn)))){
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
                m_tlref.Value.Color = m_linecolor;
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