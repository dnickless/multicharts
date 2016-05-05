using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Linear_Reg_Curve : IndicatorObject
    {
        private ISeries<Double> m_price;

        private LinearRegValue m_linearregvalue1;

        private VariableSeries<Double> m_linreg;

        private IPlotObject Plot1;

        public Linear_Reg_Curve(object ctx) :
            base(ctx){
            length = 9;
        }

        private ISeries<Double> price{
            get { return m_price; }
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int displace { get; set; }

        protected override void Create(){
            m_linearregvalue1 = new LinearRegValue(this);
            m_linreg = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("LinReg", 0, Color.Yellow,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_price = Bars.Close;
            m_linearregvalue1.price = price;
            m_linearregvalue1.length = length;
            m_linearregvalue1.tgtbar = 0;
        }


        protected override void CalcBar(){
            if (((displace >= 0)
                 || Bars.CurrentBar > Math.Abs(displace))){
                m_linreg.Value = m_linearregvalue1[0];
                Plot1.Set(displace, m_linreg.Value);
                if ((displace <= 0)){
                    if (((PublicFunctions.DoubleGreater(price[0], m_linreg.Value) &&
                          PublicFunctions.DoubleGreater(m_linreg.Value, m_linreg[1]))
                         && PublicFunctions.DoubleLessEquals(m_linreg[1], m_linreg[2]))){
                        Alerts.Alert("Indicator turning up");
                    }
                    else{
                        if (((PublicFunctions.DoubleLess(price[0], m_linreg.Value) &&
                              PublicFunctions.DoubleLess(m_linreg.Value, m_linreg[1]))
                             && PublicFunctions.DoubleGreaterEquals(m_linreg[1], m_linreg[2]))){
                            Alerts.Alert("Indicator turning down");
                        }
                    }
                }
            }
        }
    }
}