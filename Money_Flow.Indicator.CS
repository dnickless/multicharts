using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Money_Flow : IndicatorObject
    {
       
        private VariableSeries<Double> m_moneyflowval;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        public Money_Flow(object ctx) :
            base(ctx){
            overbcolor = Color.Red;
            overscolor = Color.Cyan;
            overbought = 80;
            oversold = 20;
            alertlength = 14;
            length = 14;
        }

        [Input]
        public int length { get; set; }

        [Input]
        public int alertlength { get; set; }

        [Input]
        public double oversold { get; set; }

        [Input]
        public double overbought { get; set; }

        [Input]
        public Color overscolor { get; set; }

        [Input]
        public Color overbcolor { get; set; }

        protected override void Create(){
            m_moneyflowval = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("MoneyFlow", 0, ColorTranslator.FromWin32(10789024),
                                           Color.Empty, 0, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("OverBot", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
            Plot3 =
                AddPlot(new PlotAttributes("OverSld", 0, Color.Green,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
        }


        protected override void CalcBar(){
            m_moneyflowval.Value = Bars.MoneyFlow(length);
            Plot1.Set(0, m_moneyflowval.Value);
            Plot2.Set(0, overbought);
            Plot3.Set(0, oversold);
            if (PublicFunctions.DoubleGreater(m_moneyflowval.Value, overbought)){
                Plot1.Colors[0] = overbcolor;
            }
            else{
                if (PublicFunctions.DoubleLess(m_moneyflowval.Value, oversold)){
                    Plot1.Colors[0] = overscolor;
                }
            }
            if (((Bars.Close.LowestBar(alertlength) == 0)
                 && PublicFunctions.DoubleGreater(m_moneyflowval.LowestBar(alertlength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(alertlength) == 0)
                     && PublicFunctions.DoubleGreater(m_moneyflowval.HighestBar(alertlength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}