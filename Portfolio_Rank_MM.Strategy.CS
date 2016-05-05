using System.Linq;
using PowerLanguage.Function;
using System.Collections.Generic;


namespace PowerLanguage.Strategy {
	public class Portfolio_Rank_MM : PortfolioSignalObject {

		[Input]
		public bool UsePercentFromPortfolioForEntry { get; set; }

		[Input]
		public double PortfolioBalancePercent { get; set; }

		[Input]
		public int StdDevLength { get; set; }
		
		[Input]
		public int BuyBestX { get; set; }
		[Input]
		public int SellWorstY { get; set; }

		[Input]
		public bool TraceOutput { get; set; }

		private VariableSeries<double> avgReturn;

		public Portfolio_Rank_MM(object _ctx) : base(_ctx) {
			UsePercentFromPortfolioForEntry = false;
			PortfolioBalancePercent = 1;
			StdDevLength = 14;
			BuyBestX = 10;
			SellWorstY = 10;
			TraceOutput = false;
		}

		protected override void Create() {
			avgReturn = new VariableSeries<double>(this);
		}

		protected override void StartCalc() {
			if (Environment.ApplicationCode != EApplicationCode.Portfolio)
				ExecControl.Abort("Portfolio Rank Money Management Signal can be applied for MCPortfolio application only.");
			if (PortfolioStrategies.Count < BuyBestX + SellWorstY)
				ExecControl.Abort("Portfolio Rank Monem Management Signal, please check inputs, BuyBestX + SellWorstY should be less or equal to tradeable Instruments number");
		}

		protected override void CalcBar() {
			this.StrategiesDenyEntriesAll();

			int strategiesCount = PortfolioStrategies.Count;
			avgReturn.Value = 0;

			var ranks = new Dictionary<int, double>();
			for (int idx = 0; idx < strategiesCount; idx++) {
				ranks[idx] = PortfolioStrategies[idx].PortfolioData["RankStrategyR"].safe_cast2double();
				avgReturn.Value += ranks[idx];
			}

			avgReturn.Value /= strategiesCount;
			double standardDeviation = avgReturn.StandardDeviationCustom(StdDevLength, 1);
			if (standardDeviation == 0) standardDeviation = 1;

			ranks = ranks.ToDictionary(x => x.Key, y => (y.Value - avgReturn.Value) / standardDeviation);

			ranks = ranks.OrderByDescending(elem => elem.Value).ToDictionary(x => x.Key, y => y.Value);

			List<int> strategies = new List<int>();
			int inLong = this.StrategiesInLong(ref strategies);
			for (int idx = 0; idx < BuyBestX - inLong; idx++) {
				PortfolioStrategies[ranks.Keys.ElementAt(idx)].AllowEntriesLong = true;
				if (UsePercentFromPortfolioForEntry)
					PortfolioStrategies[ranks.Keys.ElementAt(idx)].EntryContracts =
						this.CalcContractsForEntry(PortfolioBalancePercent, ranks.Keys.ElementAt(idx));

				if (TraceOutput)
					Output.WriteLine("CurrentBar = {0}. Allow LONG for symbol {1}, Contracts = {2}",
						Bars.CurrentBar, PortfolioStrategies[ranks.Keys.ElementAt(idx)].Signals[0].Bars.Info.Name,
						PortfolioStrategies[ranks.Keys.ElementAt(idx)].EntryContracts);
			}

			int inShort = this.StrategiesInShort(ref strategies);
			for (int idx = strategiesCount - 1; idx > strategiesCount - SellWorstY + inShort + 1; idx--) {
				PortfolioStrategies[ranks.Keys.ElementAt(idx)].AllowEntriesShort = true;
				if (UsePercentFromPortfolioForEntry)
					PortfolioStrategies[ranks.Keys.ElementAt(idx)].EntryContracts =
						this.CalcContractsForEntry(PortfolioBalancePercent, ranks.Keys.ElementAt(idx));

				if (TraceOutput)
					Output.WriteLine("CurrentBar = {0}. Allow SHORT for symbol {1}, Contracts = {2}",
						Bars.CurrentBar, PortfolioStrategies[ranks.Keys.ElementAt(idx)].Signals[0].Bars.Info.Name,
						PortfolioStrategies[ranks.Keys.ElementAt(idx)].EntryContracts);
			}
		}
	}
}