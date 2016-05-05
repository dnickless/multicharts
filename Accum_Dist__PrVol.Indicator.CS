using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    public class Accum_Dist__PrVol : IndicatorObject
    {
        private AccumulationDistribution m_accumdist1;
        private VariableSeries<Double> m_accdstprvol;

        private IPlotObject Plot1;

        public Accum_Dist__PrVol(object ctx) :
            base(ctx){
            AlertLength = 14;
        }

        [Input]
        public int AlertLength { get; set; }

        protected override void Create(){
            m_accumdist1 = new AccumulationDistribution(this);
            m_accdstprvol = new VariableSeries<Double>(this);
            Plot1 =
                AddPlot(new PlotAttributes("AccDst-PrVol", 0, Color.Blue,
                                           Color.Empty, 0, 0, true));
        }

        protected override void StartCalc(){
            m_accumdist1.AnyVolume = Bars.TrueVolume();
        }


        protected override void CalcBar()
        {
            m_accdstprvol.Value = m_accumdist1[0];
            Plot1.Set(0, m_accdstprvol.Value);
            if ((( Bars.Close.LowestBar(AlertLength) == 0)
                 && PublicFunctions.DoubleGreater(m_accdstprvol.LowestBar(AlertLength), 0)))
            {
                Alerts.Alert("Bullish divergence - new low not confirmed");
            }
            else{
                if (((Bars.Close.HighestBar(AlertLength) == 0)
                     && PublicFunctions.DoubleGreater(m_accdstprvol.HighestBar(AlertLength), 0)))
                {
                    Alerts.Alert("Bearish divergence - new high not confirmed");
                }
            }
        }
    }
}