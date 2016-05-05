using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class New_Intraday_HiLo : IndicatorObject
    {
        private RS_DailyDataArray m_rs_dailydataarray1;

        private BarNumber m_barnumber1;

        private RS_Extremes m_rs_extremes1;

        private RS_Average m_rs_average1;

        private RS_Average m_rs_average2;

        private RS_Average m_rs_average3;

        private VariableObject<Int32> m_index;

        private VariableSeries<Boolean> m_qualhi;

        private VariableSeries<Boolean> m_quallo;

        private VariableObject<Double> m_oprevhighest;

        private VariableObject<Double> m_oprevhighestday;

        private VariableObject<Double> m_oprevlowest;

        private VariableObject<Double> m_oprevlowestday;

        private VariableObject<Boolean> m_revgapup;

        private VariableObject<Boolean> m_revgapdn;

        private Array2DSimple<Double> m_dataarray;

        private ArraySimple<Double> m_subarray;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public New_Intraday_HiLo(object ctx) :
            base(ctx){
            endbartime = new DateTime(1, 1, 1, 14, 30, 0);
            startbartime = new DateTime(1, 1, 1, 10, 0, 0);
            barsbetween = 10;
            requirereversalgap = true;
            numdays = 3;
        }

        [Input]
        public int numdays { get; set; }

        [Input]
        public bool requirereversalgap { get; set; }

        [Input]
        public double barsbetween { get; set; }

        [Input]
        public DateTime startbartime { get; set; }

        [Input]
        public DateTime endbartime { get; set; }

        protected override void Create(){
            m_rs_dailydataarray1 = new RS_DailyDataArray(this);
            m_barnumber1 = new BarNumber(this);
            m_rs_extremes1 = new RS_Extremes(this);
            m_rs_average1 = new RS_Average(this);
            m_rs_average2 = new RS_Average(this);
            m_rs_average3 = new RS_Average(this);
            m_index = new VariableObject<Int32>(this);
            m_qualhi = new VariableSeries<Boolean>(this);
            m_quallo = new VariableSeries<Boolean>(this);
            m_oprevhighest = new VariableObject<Double>(this);
            m_oprevhighestday = new VariableObject<Double>(this);
            m_oprevlowest = new VariableObject<Double>(this);
            m_oprevlowestday = new VariableObject<Double>(this);
            m_revgapup = new VariableObject<Boolean>(this);
            m_revgapdn = new VariableObject<Boolean>(this);
            m_dataarray = new Array2DSimple<Double>(this, 13, 101);
            m_subarray = new ArraySimple<Double>(this, 4);
            Plot1 =
                AddPlot(new PlotAttributes("NewHiLo", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 5, 0,
                                           true));
            Plot2 =
                AddPlot(new PlotAttributes("RevGap", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 2,
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
            m_rs_dailydataarray1.numdays = numdays + 2;
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
            m_rs_average1.rowtoavg = 6;
            m_rs_average1.numdays = numdays;
            m_rs_average1.offset = 2;
            m_rs_average1.dataarray = m_dataarray;
            m_rs_average1.index = m_index;
            m_rs_average2.rowtoavg = 6;
            m_rs_average2.numdays = numdays;
            m_rs_average2.offset = 1;
            m_rs_average2.dataarray = m_dataarray;
            m_rs_average2.index = m_index;
            m_rs_average3.rowtoavg = 6;
            m_rs_average3.numdays = numdays;
            m_rs_average3.offset = 0;
            m_rs_average3.dataarray = m_dataarray;
            m_rs_average3.index = m_index;
            m_dataarray.DefaultValue = 0;
            m_subarray.DefaultValue = 0;
        }


        protected override void CalcBar(){
            m_rs_dailydataarray1.Call();
            var m_barnum = m_barnumber1[0];
            var m_barssincedayhi = (m_barnum - m_dataarray[0][10, m_index.Value]);
            var m_barssincedaylo = (m_barnum - m_dataarray[0][11, m_index.Value]);
            var mBbminus1 = (barsbetween - 1);
            m_qualhi.Value = PublicFunctions.DoubleGreaterEquals(m_barssincedayhi, mBbminus1);
            m_quallo.Value = PublicFunctions.DoubleGreaterEquals(m_barssincedaylo, mBbminus1);
            if (requirereversalgap){
                if (((Bars.CurrentBar == 1)
                     || (Bars.Time[0].Date != Bars.Time[1].Date))){
                    m_rs_extremes1.Call();
                    var m_gapup = PublicFunctions.DoubleGreater(m_dataarray[0][((1)), ((m_index.Value))],
                                                            m_oprevhighest.Value);
                    var m_gapdn = PublicFunctions.DoubleLess(m_dataarray[0][((1)), ((m_index.Value))], m_oprevlowest.Value);
                    var m_thirdprevavg = m_rs_average1[0];
                    var m_secondprevavg = m_rs_average2[0];
                    var m_prevavg = m_rs_average3[0];
                    var m_uptrend = (PublicFunctions.DoubleGreater(m_prevavg, m_secondprevavg) &&
                                 PublicFunctions.DoubleGreater(m_secondprevavg, m_thirdprevavg));
                    var m_dntrend = (PublicFunctions.DoubleLess(m_prevavg, m_secondprevavg) &&
                                 PublicFunctions.DoubleLess(m_secondprevavg, m_thirdprevavg));
                    m_revgapup.Value = (m_dntrend && m_gapup);
                    m_revgapdn.Value = (m_uptrend && m_gapdn);
                }
            }
            else{
                m_revgapup.Value = true;
                m_revgapdn.Value = true;
            }
            if ((Bars.Time[0].Date == Bars.Time[1].Date))
            {
                var m_newdayhi = (m_barssincedayhi == 0 && m_qualhi[1] && m_revgapup.Value);
                var m_newdaylo = (m_barssincedaylo == 0 && m_quallo[1] && m_revgapdn.Value);
                if ((m_newdayhi && m_newdaylo)){
                    Plot1.Set(0, Bars.Close[0], Color.White);
                }
                else{
                    if (m_newdayhi){
                        Plot1.Set(0, Bars.High[0], Color.Yellow);
                    }
                    else{
                        if (m_newdaylo){
                            Plot1.Set(0, Bars.Low[0], Color.Cyan);
                        }
                    }
                }
            }
            var _now_time = new DateTime(1, 1, 1, Bars.Time[0].Hour, Bars.Time[0].Minute, Bars.Time[0].Second);

            if (_now_time < startbartime || _now_time > endbartime){
                Plot1.Colors[0] = Color.DarkGray;
            }

            if (requirereversalgap){
                if (m_revgapup.Value){
                    Plot2.Set(0, Bars.Close[0], Color.Blue);
                }
                else{
                    if (m_revgapdn.Value){
                        Plot2.Set(0, Bars.Close[0], default(Color));
                    }
                }
            }
            Plot3.Set(0, m_dataarray[0][((2)), ((m_index.Value))]);
            Plot4.Set(0, m_dataarray[0][((3)), ((m_index.Value))]);
            if (m_qualhi.Value){
                Plot3.Widths[0] = 2;
            }
            if (m_quallo.Value){
                Plot4.Widths[0] = 2;
            }
        }
    }
}