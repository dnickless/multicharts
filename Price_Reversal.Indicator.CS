using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class Price_Reversal : IndicatorObject
    {
        private RS_DailyDataArray m_rs_dailydataarray1;

        private RS_TrueExtremes m_rs_trueextremes1;

        private RS_PriceExtension m_rs_priceextension1;

        private RS_ReversalPatterns m_rs_reversalpatterns1;

        private BarNumber m_barnumber1;

        private VariableObject<int> m_value3;

        private VariableObject<Int32> m_index;

        private VariableObject<Double> m_prevtrhighest;

        private VariableObject<Double> m_prevtrlowest;

        private VariableObject<Double> m_prevatr;

        private VariableObject<int> m_osetup;

        private VariableObject<Double> m_otrigger;

        private Array2DSimple<Double> m_dataarray;

        private ArraySimple<Double> m_subarray;

        private IPlotObject Plot1;

        private IPlotObject Plot2;

        private IPlotObject Plot3;

        private IPlotObject Plot4;

        public Price_Reversal(object ctx) :
            base(ctx){
            lolinecolor = Color.LightGray;
            hilinecolor = Color.Magenta;
            gapsizefactor = 0.2;
            revcriteria = 1;
            finalrangefactor = 1;
            minconsolindex = 2.25;
            numdays = 3;
        }

        [Input]
        public int numdays { get; set; }

        [Input]
        public double minconsolindex { get; set; }

        [Input]
        public double finalrangefactor { get; set; }

        [Input]
        public int revcriteria { get; set; }

        [Input]
        public double gapsizefactor { get; set; }

        [Input]
        public Color hilinecolor { get; set; }

        [Input]
        public Color lolinecolor { get; set; }

        protected override void Create(){
            m_rs_dailydataarray1 = new RS_DailyDataArray(this);
            m_rs_trueextremes1 = new RS_TrueExtremes(this);
            m_rs_priceextension1 = new RS_PriceExtension(this);
            m_rs_reversalpatterns1 = new RS_ReversalPatterns(this);
            m_barnumber1 = new BarNumber(this);
            m_value3 = new VariableObject<int>(this);
            m_index = new VariableObject<Int32>(this);
            m_prevtrhighest = new VariableObject<Double>(this);
            m_prevtrlowest = new VariableObject<Double>(this);
            m_prevatr = new VariableObject<Double>(this);
            m_osetup = new VariableObject<int>(this);
            m_otrigger = new VariableObject<Double>(this);
            m_dataarray = new Array2DSimple<Double>(this, 13, 101);
            m_subarray = new ArraySimple<Double>(this, 4);
            Plot1 =
                AddPlot(new PlotAttributes("Ext", EPlotShapes.Point, Color.Blue,
                                           Color.Empty, 3, 0, true));
            Plot2 =
                AddPlot(new PlotAttributes("Setup", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 4,
                                           0,
                                           true));
            Plot3 =
                AddPlot(new PlotAttributes("Trigger", EPlotShapes.Point,
                                           Color.Cyan, Color.Empty, 5, 0,
                                           true));
            Plot4 =
                AddPlot(new PlotAttributes("HiLoLines", EPlotShapes.Point,
                                           Color.Yellow, Color.Empty, 1,
                                           0,
                                           true));
        }

        protected override void StartCalc(){
            m_rs_dailydataarray1.numdays = numdays;
            m_rs_dailydataarray1.odataarray = m_dataarray;
            m_rs_dailydataarray1.oindex = m_index;
            m_rs_dailydataarray1.osubarray = m_subarray;
            m_rs_trueextremes1.numdays = numdays;
            m_rs_trueextremes1.dataarray = m_dataarray;
            m_rs_trueextremes1.index = m_index;
            m_rs_trueextremes1.oprevtrhighest = m_prevtrhighest;
            m_rs_trueextremes1.oprevtrlowest = m_prevtrlowest;
            m_rs_trueextremes1.oprevatr = m_prevatr;
            m_rs_priceextension1.numdays = numdays;
            m_rs_priceextension1.minconsolindex = minconsolindex;
            m_rs_priceextension1.finalrangefactor = finalrangefactor;
            m_rs_priceextension1.prevtrhighest = m_prevtrhighest;
            m_rs_priceextension1.prevtrlowest = m_prevtrlowest;
            m_rs_priceextension1.prevatr = m_prevatr;
            m_rs_priceextension1.dataarray = m_dataarray;
            m_rs_priceextension1.index = m_index;
            m_rs_reversalpatterns1.revdirection = m_value3;
            m_rs_reversalpatterns1.revcriteria = revcriteria;
            m_rs_reversalpatterns1.gapsizefactor = gapsizefactor;
            m_rs_reversalpatterns1.dataarray = m_dataarray;
            m_rs_reversalpatterns1.index = m_index;
            m_rs_reversalpatterns1.subarray = m_subarray;
            m_rs_reversalpatterns1.osetup = m_osetup;
            m_rs_reversalpatterns1.otrigger = m_otrigger;
        }


        protected override void CalcBar(){
            m_rs_dailydataarray1.Call();
            if (((Bars.CurrentBar == 1)
                 || (Bars.Time[0].Date != Bars.Time[1].Date)))
            {
                m_rs_trueextremes1.Call();
                m_value3.Value = m_rs_priceextension1[0];
            }
            m_rs_reversalpatterns1.Call();
            if ((m_value3.Value == 1)){
                Plot1.Set(0, Bars.High[0]);
                if ((m_osetup.Value == 1)){
                    Plot2.Set(0, Bars.Close[0]);
                    if (PublicFunctions.DoubleGreater(m_otrigger.Value, 0)){
                        Plot3.Set(0, Bars.Low[0]);
                    }
                }
            }
            else{
                if ((m_value3.Value == 2)){
                    Plot1.Set(0, Bars.Low[0]);
                    if ((m_osetup.Value == 1)){
                        Plot2.Set(0, Bars.Close[0]);
                        if (PublicFunctions.DoubleGreater(m_otrigger.Value, 0)){
                            Plot3.Set(0, Bars.High[0]);
                        }
                    }
                }
            }
            if ((m_barnumber1[0]%2 == 0)){
                Plot4.Set(0, m_prevtrhighest.Value, hilinecolor);
            }
            else{
                Plot4.Set(0, m_prevtrlowest.Value, lolinecolor);
            }
        }
    }
}