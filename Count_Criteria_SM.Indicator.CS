using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Count_Criteria_SM : IndicatorObject
    {
        private Func<int,bool> m_criteria;

        private ITextObject m_textid;

        private IPlotObject Plot1;

        public Count_Criteria_SM(object ctx) :
            base(ctx){
            textcolor = Color.Cyan;
            occur = 7;
            length = 14;
            showtext = true;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public double occur { get; set; }

        [Input]
        public bool showtext { get; set; }

        [Input]
        public Color textcolor { get; set; }

        protected override void Create(){
            Plot1 =
                AddPlot(new PlotAttributes("CntCrtSM", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 3, 0,
                                           true));
        }

        protected override void StartCalc(){
            m_criteria = _bb => PublicFunctions.DoubleGreater(Bars.Close[_bb], Bars.High[(1 + _bb)]);
        }


        protected override void CalcBar(){
            if (m_criteria(0))
            {
                var m_count = m_criteria.CountIF(length);
                if (PublicFunctions.DoubleGreaterEquals(m_count, occur)){
                    Plot1.Set(0, Bars.High[0]);
                    Alerts.Alert();
                    if (showtext){
                        m_textid = DrwText.Create(new ChartPoint(Bars.Time[0], Bars.High[0]), m_count.ToString());
                        m_textid.HStyle = ETextStyleH.Center;
                        m_textid.VStyle = ETextStyleV.Above;
                        m_textid.Color = textcolor;
                    }
                }
            }
            else{
                Plot1.Reset();
            }
        }
    }
}