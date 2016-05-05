using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class VWAP_reset : IndicatorObject
    {
        private vwapResettable m_vwapresettable1;

        private VariableSeries<Double> m_price;
        private VariableSeries<Double> m_it;
        private VariableObject<Color> m_color;

        private IPlotObject Plot1;

        public VWAP_reset(object ctx) :
            base(ctx){
            dncolor = Color.Magenta;
            upcolor = Color.Cyan;
            time5 = new DateTime(1, 1, 1, 14, 15, 0);
            time4 = new DateTime(1, 1, 1, 13, 15, 0);
            time3 = new DateTime(1, 1, 1, 11, 29, 0);
            time2 = new DateTime(1, 1, 1, 9, 30, 0);
            time1 = new DateTime(1, 1, 1, 9, 0, 0);
        }

        [Input]
        public int localhrsoffset { get; set; }

        [Input]
        public DateTime time1 { get; set; }

        [Input]
        public DateTime time2 { get; set; }

        [Input]
        public DateTime time3 { get; set; }

        [Input]
        public DateTime time4 { get; set; }

        [Input]
        public DateTime time5 { get; set; }

        [Input]
        public Color upcolor { get; set; }

        [Input]
        public Color dncolor { get; set; }

        protected override void Create(){
            m_vwapresettable1 = new vwapResettable(this);
            m_price = new VariableSeries<Double>(this);
            m_it = new VariableSeries<Double>(this);
            m_color = new VariableObject<Color>(this);
            Plot1 =
                AddPlot(new PlotAttributes("vwap_reset", EPlotShapes.Point,
                                           Color.Blue, Color.Empty, 0, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_vwapresettable1.price = m_price;
            m_vwapresettable1.localhrsoffset = localhrsoffset;
            m_vwapresettable1.time1 = time1;
            m_vwapresettable1.time2 = time2;
            m_vwapresettable1.time3 = time3;
            m_vwapresettable1.time4 = time4;
            m_vwapresettable1.time5 = time5;
            m_color.DefaultValue = Color.Yellow;
        }


        protected override void CalcBar(){
            m_price.Value = Bars.AvgPrice(0);
            m_it.Value = m_vwapresettable1[0];
            Plot1.Set(0, m_it.Value);
            if (PublicFunctions.DoubleGreater(m_it.Value, m_it[1])){
                m_color.Value = upcolor;
            }
            if (PublicFunctions.DoubleLess(m_it.Value, m_it[1])){
                m_color.Value = dncolor;
            }
            Plot1.Colors[0] = m_color.Value;
        }
    }
}