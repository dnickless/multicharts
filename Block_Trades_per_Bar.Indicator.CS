using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator{
	public class Block_Trades_per_Bar : IndicatorObject {

		public Block_Trades_per_Bar(object _ctx):base(_ctx){
            datanum = 1;
            blocksize = 5000;
		}

		private VariableObject<Double> m_blocktrades;
        private VariableObject<int> m_barnum;
		private IPlotObject plot1;
	
        [Input]
        public double blocksize { get; set; }

        [Input]
        public int datanum { get; set; }

        protected override void Create(){
            m_blocktrades = new VariableObject<Double>(this);
            m_barnum = new VariableObject<int>(this);

			plot1 = AddPlot(new PlotAttributes("BlockTrds", EPlotShapes.Histogram,
                    	Color.Blue, Color.Empty, 1, 0, true));
        }

        protected override void CalcBar(){
            if ((Bars.LastBarOnChart && (BarsOfData(datanum).Status != EBarState.Close))){
                if ((m_barnum.Value != Bars.CurrentBar)){
                    m_blocktrades.Value = 0;
                    m_barnum.Value = Bars.CurrentBar;
                }
                if (PublicFunctions.DoubleGreaterEquals(Bars.StatusLine.LastVolume, blocksize)){
                    m_blocktrades.Value = (m_blocktrades.Value + 1);
                }
                plot1.Set(0, m_blocktrades.Value);
            }
        }
	}
}