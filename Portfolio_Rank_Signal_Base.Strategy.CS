using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class Portfolio_Rank_Signal_Base : PortfolioSignalObject {
		[Input]
		public int BasedOnData { get; set; }
		
		[Input]
		public bool TraceOutput { get; set; }
		
		public Portfolio_Rank_Signal_Base(object _ctx) : base(_ctx) {
			BasedOnData = 2;
			TraceOutput = false;
		}
		
		private IOrderMarket buy;
		private IOrderMarket sellshort;

		private VariableSeries<int> barNumber;

		protected override void Create() {
			buy = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "LE", EOrderAction.Buy));
			sellshort = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "SE", EOrderAction.SellShort));
			barNumber =  new VariableSeries<int>(this);
		}

		protected override void StartCalc() {
			CommonPortfolioData["RankStrategyApplied"] = "true";
			
			if (!IsExist(BasedOnData))
				ExecControl.Abort("Portfolio Rank Signal Base needs datastream {0}." + BasedOnData);
			if (Environment.ApplicationCode != EApplicationCode.Portfolio)
				ExecControl.Abort("Portfolio Rank Signal Base can be applied for MCPortfolio application only.");

			if (TraceOutput) Output.Clear();
		}

		protected override void CalcBar() {
			barNumber.Value = BarsOfData(BasedOnData).CurrentBar;

			if (barNumber.Value > barNumber[1]) {
				double rank = Formula(this);
				MyPortfolioData["RankStrategyR"] = rank;

				if (TraceOutput)
					Output.WriteLine("CurrentBar = {0}. Put MyIndicator value = {1} for symbol {2}.", Bars.CurrentBar, rank, Bars.Info.Name);
			}
			
			buy.Send();
			sellshort.Send();

			// money management
			double moneyCostForInvestPerCntrct = this.CalcMoneyForEntryPerCntrct(Bars.CloseValue, Portfolio.MarginPerContract) + 
				this.CalcMoneyForEntryPerCntrct(Bars.CloseValue, Portfolio.MaxPotentialLossPerContract);

			if ( moneyCostForInvestPerCntrct < 0 )
				ExecControl.Abort("Error! Price = {0}, PMargin = {1}, PMaxPLoss = {2}", Bars.CloseValue, Portfolio.MarginPerContract, Portfolio.MaxPotentialLossPerContract);

			MyPortfolioData[PortfolioHelpFunctions.MoneyCostForInvestPerCtrct] = this.FromSymbolToPortfolioCurrency(moneyCostForInvestPerCntrct);
		}

		private double Formula(IStudy _this) {
			IInstrument symbol = _this.BarsOfData(BasedOnData);
			return (symbol.CloseValue - symbol.Close[1]) / symbol.CloseValue;
		}
	}
}