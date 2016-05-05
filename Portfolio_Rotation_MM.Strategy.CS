using System.Linq;
using PowerLanguage.Function;
using System.Collections.Generic;

namespace PowerLanguage.Strategy {
	public class Portfolio_Rotation_MM : PortfolioSignalObject {

		[Input]
        public int BuyBestX { get; set; }
		[Input]
        public int SellWorstY { get; set; }
		[Input]
		public bool UsePortfolioMoneyPcnt { get; set; }
		[Input]
		public double PortfolioMoneyPcntForEntry { get; set; }
		[Input]
		public bool TraceOutput { get; set; }

		public Portfolio_Rotation_MM(object _ctx) : base(_ctx) {
			BuyBestX = 10;
			SellWorstY = 10;
			UsePortfolioMoneyPcnt = false;
			PortfolioMoneyPcntForEntry = 1;
			TraceOutput = false;
		}

		private Dictionary<int, double> bestStrategies = new Dictionary<int, double>();
		private Dictionary<int, double> worstStrategies = new Dictionary<int, double>();

		protected override void StartCalc() {
			if (TraceOutput) Output.Clear();
		}

		protected override void CalcBar() {
			if (Environment.ApplicationCode != EApplicationCode.Portfolio)
				ExecControl.Abort("Signal can be applied (as money management) in portfolio only.");
            if (PortfolioStrategies.Count < BuyBestX + SellWorstY)
                ExecControl.Abort(
                    "Portfolio has no enough instruments: instruments number = {0}; BuyBestX = {1}; SellWorstY = {2}",
                    PortfolioStrategies.Count, BuyBestX, SellWorstY);
			
			this.StrategiesDenyEntriesAll();

			bestStrategies.Clear();
			worstStrategies.Clear();

			for (int idx = 0; idx < PortfolioStrategies.Count; idx++) {
                double value = PortfolioStrategies[idx].PortfolioData["RotationalValue"].safe_cast2double();
				bestStrategies.Add(idx, value);
				worstStrategies.Add(idx, value);
				if (TraceOutput)
					Output.WriteLine("{0}. For {1} strategy value = {2}", Bars.CurrentBar, idx, value);
			}

		    bestStrategies = bestStrategies.OrderByDescending(elem => elem.Value).ToDictionary(x => x.Key, y => y.Value);
		    worstStrategies = worstStrategies.OrderBy(elem => elem.Value).ToDictionary(x => x.Key, y => y.Value);

			if (TraceOutput)
				foreach(var bestElem in bestStrategies)
					Output.WriteLine("{0}. Best = {1}:{2}.", Bars.CurrentBar, bestElem.Key, bestElem.Value);

			var inLongStrategies = new List<int>();
			this.StrategiesInLong(ref inLongStrategies);

			for (int idx = 0; idx < BuyBestX; idx++)
				if (!inLongStrategies.Contains(bestStrategies.Keys.ElementAt(idx)))
					PortfolioStrategies[bestStrategies.Keys.ElementAt(idx)].AllowEntriesLong = true;

			var inShortStrategies = new List<int>();
			this.StrategiesInShort(ref inShortStrategies);

			for (int idx = 0; idx < SellWorstY; idx++)
				if (!inShortStrategies.Contains(worstStrategies.Keys.ElementAt(idx)))
					PortfolioStrategies[worstStrategies.Keys.ElementAt(idx)].AllowEntriesShort = true;

			// money management
			if (UsePortfolioMoneyPcnt) {
				for (int idx = 0; idx < BuyBestX; idx++)
					PortfolioStrategies[bestStrategies.Keys.ElementAt(idx)].EntryContracts =
						this.CalcContractsForEntry(PortfolioMoneyPcntForEntry, bestStrategies.Keys.ElementAt(idx));

				for (int idx = 0; idx < SellWorstY; idx++)
					PortfolioStrategies[worstStrategies.Keys.ElementAt(idx)].EntryContracts =
						this.CalcContractsForEntry(PortfolioMoneyPcntForEntry, worstStrategies.Keys.ElementAt(idx));
			}
		}
	}
}