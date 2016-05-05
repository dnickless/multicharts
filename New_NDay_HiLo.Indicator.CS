using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_NDay_HiLo : IndicatorObject
    {
        private RS_DailyDataArray m_rs_dailydataarray1;

        private RS_Extremes m_rs_extremes1;

        private VariableObject<Int32> m_index;

        private VariableObject<Double> m_oprevhighest;

        private VariableObject<Double> m_oprevhighestday;

        private VariableObject<Double> m_oprevlowest;

        private VariableObject<Double> m_oprevlowestday;

        private VariableObject<Boolean> m_qualhi;

        private VariableObject<Boolean> m_quallo;

        private VariableObject<DateTime> m_prevnewhidate;

        private VariableObject<DateTime> m_prevnewlodate;

        private Array2DSimple<Double> m_dataarray;

        private ArraySimple<Double> m_subarray;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public New_NDay_HiLo(object ctx) :
            base(ctx){
            numdays = 7;
        }

        [Input]
        public int numdays { get; set; }

        [Input]
        public int newqualdays { get; set; }

        [Input]
        public int oldqualdays { get; set; }

        protected override void Create(){
            m_rs_dailydataarray1 = new RS_DailyDataArray(this);
            m_rs_extremes1 = new RS_Extremes(this);
            m_index = new VariableObject<Int32>(this);
            m_oprevhighest = new VariableObject<Double>(this);
            m_oprevhighestday = new VariableObject<Double>(this);
            m_oprevlowest = new VariableObject<Double>(this);
            m_oprevlowestday = new VariableObject<Double>(this);
            m_qualhi = new VariableObject<Boolean>(this);
            m_quallo = new VariableObject<Boolean>(this);
            m_prevnewhidate = new VariableObject<DateTime>(this);
            m_prevnewlodate = new VariableObject<DateTime>(this);
            m_dataarray = new Array2DSimple<Double>(this, 13, 101);
            m_subarray = new ArraySimple<Double>(this, 4);
            Plot1 =
                AddPlot(new PlotAttributes("NewNDayHi", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 4, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("NewNDayLo", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("HiLine", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
            Plot4 =
                AddPlot(new PlotAttributes("LoLine", 0, Color.Magenta,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_rs_dailydataarray1.numdays = numdays;
            m_rs_dailydataarray1.odataarray = m_dataarray;
            m_rs_dailydataarray1.oindex = m_index;
            m_rs_dailydataarray1.osubarray = m_subarray;
            m_rs_extremes1.numdays = numdays;
            m_rs_extremes1.dataarray = m_dataarray;
            m_rs_extremes1.index = m_index;
            m_rs_extremes1.oprevhighest = m_oprevhighest;
            m_rs_extremes1.oprevhighestday = m_oprevhighestday;
            m_rs_extremes1.oprevlowest = m_oprevlowest;
            m_rs_extremes1.oprevlowestday = m_oprevlowestday;
            m_prevnewhidate.DefaultValue = DateTime.MinValue;
            m_prevnewlodate.DefaultValue = DateTime.MinValue;
        }


        protected override void CalcBar(){
            m_rs_dailydataarray1.Call();
            if (Bars.CurrentBar == 1 || Bars.Time[0].Date != Bars.Time[1].Date)
            {
                m_rs_extremes1.Call();
                var m_startday = (numdays - oldqualdays);
                m_qualhi.Value = (PublicFunctions.DoubleLessEquals(m_oprevhighestday.Value, m_startday) &&
                                  PublicFunctions.DoubleGreater(m_oprevhighestday.Value, newqualdays));
                m_quallo.Value = (PublicFunctions.DoubleLessEquals(m_oprevlowestday.Value, m_startday) &&
                                  PublicFunctions.DoubleGreater(m_oprevlowestday.Value, newqualdays));
            }
            if (((m_qualhi.Value && PublicFunctions.DoubleGreater(Bars.High[0], m_oprevhighest.Value))
                 &&
                 PublicFunctions.IffLogic((m_dataarray[0][((12)), ((m_index.Value))] == m_subarray[0][((3))]),
                                          PublicFunctions.DoubleLessEquals(m_subarray[0][((1))], m_oprevhighest.Value),
                                          true))){
                if ((Bars.Time[0] != m_prevnewhidate.Value)){
                    Plot1.Set(0, Bars.High[0]);
                }
                m_prevnewhidate.Value = Bars.Time[0];
            }
            else{
                if (((m_quallo.Value && PublicFunctions.DoubleLess(Bars.Low[0], m_oprevlowest.Value))
                     &&
                     PublicFunctions.IffLogic((m_dataarray[0][((12)), ((m_index.Value))] == m_subarray[0][((3))]),
                                              PublicFunctions.DoubleGreaterEquals(m_subarray[0][((2))],
                                                                                  m_oprevlowest.Value), true))){
                    if ((Bars.Time[0] != m_prevnewlodate.Value)){
                        Plot2.Set(0, Bars.Low[0]);
                    }
                    m_prevnewlodate.Value = Bars.Time[0];
                }
            }
            Plot3.Set(0, m_oprevhighest.Value);
            Plot4.Set(0, m_oprevlowest.Value);
            if (m_qualhi.Value){
                Plot3.Widths[0] = 2;
            }
            if (m_quallo.Value){
                Plot4.Widths[0] = 2;
            }
        }
    }
}