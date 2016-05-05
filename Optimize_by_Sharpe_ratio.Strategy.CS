using PowerLanguage.Function;

namespace PowerLanguage.Strategy {

	
	public class Optimize_by_Sharpe_ratio : SignalObject {
		
		private SharpeRatio m_SharpeRatio;
		
		public Optimize_by_Sharpe_ratio(object ctx) : 
				base(ctx) {
		}

	    [Input]
	    public SharpeRatio.EPeriod Period{ get; set; }
		
		protected override void Create() {
			m_SharpeRatio = new SharpeRatio(this);
		}
		
		protected override void StartCalc() {
            m_SharpeRatio.Period = Period;
		}
		
		protected override void CalcBar() {
			if (Bars.LastBarOnChart)
                CustomFitnessValue = m_SharpeRatio.Value;
		}
	}
}