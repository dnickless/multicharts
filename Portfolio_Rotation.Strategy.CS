using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
	public class Portfolio_Rotation : PortfolioSignalObject {

		private int myLength { get; set; }
		
		[Input]
		public double StopLossPcntsOfPortfolio { get; set; }
		
		[Input]
		public double ProfitTargetPcntsOfPortfolio { get; set; }

		public Portfolio_Rotation(object _ctx) : base(_ctx) {
			myLength = 14;

			StopLossPcntsOfPortfolio = .1;
			ProfitTargetPcntsOfPortfolio = .1;
		}

		private delegate double MyFormula(int length, int barsback);
		private MyFormula myFormula;

		private IOrderMarket 				buy, sell;

		protected override void Create() {
			buy = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "LE", EOrderAction.Buy));
			sell = OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "SE", EOrderAction.SellShort));
		}

		protected override void StartCalc() {
			myFormula = Bars.Close.PercentChange;
		}

		protected override void CalcBar() {
			if (Environment.ApplicationCode != EApplicationCode.Portfolio)
				ExecControl.Abort("Signal can be applied in portfolio only.");

			MyPortfolioData["RotationalValue"] = myFormula(myLength, 0);
			
			buy.Send();
			sell.Send();
	
			// money management
			double entryprice = this.EntryPrice() == 0 ? Bars.CloseValue : this.EntryPrice();
			double moneyCostForInvestPerCntrct =
				this.CalcMoneyForEntryPerCntrct(entryprice, Portfolio.MarginPerContract) +
				this.CalcMoneyForEntryPerCntrct(entryprice, Portfolio.MaxPotentialLossPerContract);

			if ( moneyCostForInvestPerCntrct < 0 )
				ExecControl.Abort("Error! Price = {0}, PMargin = {1}, PMaxPLoss = {2}", entryprice, Portfolio.MarginPerContract, Portfolio.MaxPotentialLossPerContract);

			MyPortfolioData[PortfolioHelpFunctions.MoneyCostForInvestPerCtrct] = this.FromSymbolToPortfolioCurrency(moneyCostForInvestPerCntrct);
			
			// exits
			CurSpecOrdersMode = ESpecOrdersMode.PerPosition;
			GenerateStopLoss(StopLossPcntsOfPortfolio * .01 * this.PortfolioEquity());
			GenerateProfitTarget(ProfitTargetPcntsOfPortfolio * .01 * this.PortfolioEquity());
		}
	}
}