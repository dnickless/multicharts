using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public class Portfolio_Master : PortfolioSignalObject
    {
		[Input]
        public int MaxNumberOfEntriesPerDayShortLimit { get; set; }

		[Input]
        public int MaxNumberOfEntriesPerDayLongLimit { get; set; }

		//CutlersRSIIndicatorMath cutlersRSIIndicatorMath;
		private K_RatioMath m_KRatio;
		
        public Portfolio_Master(object _ctx)
            : base(_ctx)
        {
            MaxNumberOfEntriesPerDayShortLimit = 2;
            MaxNumberOfEntriesPerDayLongLimit = 5;
        }

        protected override void Create()
        {
			//cutlersRSIIndicatorMath = new CutlersRSIIndicatorMath(this);
			m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
            if (Environment.ApplicationCode != EApplicationCode.Portfolio)
			{
                ExecControl.Abort("Signal can be applied in portfolio only.");
			}
			
            m_KRatio.NumberOfDataBarsInCalendarYear = 252;
        }

		[Input]
		public bool DoReinvestment { get; set; }
		
		private void Log(string message, params object[] parameters)
		{
			if(!Environment.Optimizing) Output.WriteLine("{0} - {1} - ({2}) - {3}: {4}", DateTime.Now, Bars.TimeValue, Environment.IsRealTimeCalc ? "REALTIME" : "HISTORIC", "MONEY MGMT", string.Format(message, parameters));
		}
		
        protected override void CalcBar()
        {
			Log("Calculating MM signal");
			
			if(Environment.Optimizing)
			{
				var result = m_KRatio.Value;
				if (Bars.LastBarOnChart)
				{
	            	CustomFitnessValue = result;
				}
			}
			//else
			{
	            var numberOfEntries = 0;
				
	            var listOfStrategiesWithOrders = new List<IPortfolioStrategy>();

				double sumCRTDR = 0.0;
				double changeInMoney = 0.0;
				
	            // let's make the most out of our available cash
	            foreach (var strategy in PortfolioStrategies)
	            {
	                if ((strategy.PortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] as double?) != null)
	                {
	                    listOfStrategiesWithOrders.Add(strategy);
	                    numberOfEntries++;
						sumCRTDR += (1.0 - (double)strategy.PortfolioData["CRTDR"]);
	                }
					
					var exit = strategy.PortfolioData["EXIT"] as bool?;
					if(exit.HasValue && exit.Value)
					{
						changeInMoney += strategy.Signals[0].CurrentPosition.OpenTrades.Sum(t => t.EntryOrder.Price * t.EntryOrder.Contracts);
					}
	            }
				
				Log("Money likely to come in due to sells: {0}", changeInMoney);

	 			//var rsi = cutlersRSIIndicatorMath.CalcNextValue(numberOfEntries);
				
	            // risk management says: Do not buy in case of two many entry signals, markets seem to be bumpy
                var allowShort = numberOfEntries <= MaxNumberOfEntriesPerDayShortLimit;
                var allowLong = numberOfEntries <= MaxNumberOfEntriesPerDayLongLimit;
				
				double sumInvested = 0.0;
				foreach (var strategy in listOfStrategiesWithOrders)
	            {
					strategy.AllowEntriesShort = allowShort;
					strategy.AllowEntriesLong = allowLong;
					var crtdr = (double)strategy.PortfolioData["CRTDR"];
					double availableCash = 0.0;
					if(DoReinvestment)
					{
						availableCash = InitialCapital + changeInMoney + Portfolio.NetProfit - Portfolio.InvestedCapital;
					}
					else
					{
						// do not reinvest potential profits, cap @ InitialCapital
						availableCash = Math.Min(InitialCapital, InitialCapital + changeInMoney - Portfolio.InvestedCapital);
					}
					var cashToInvest = availableCash / 100.0 * Portfolio.MaxRiskEquityPerPosPercent * (sumCRTDR == 0.0 ? 1.0 / (double)numberOfEntries : ((1.0 - crtdr) / sumCRTDR));
					sumInvested += cashToInvest;
					var possibleEntryPrice = Math.Abs(strategy.Signals[0].Bars.CloseValue);
	                strategy.EntryContracts = (int)(cashToInvest / possibleEntryPrice);
					Log("({0} + {1} - {2}) / 100.0 * {3} * {4} = {5}$ --> {6}# @ assumed price {7}$", InitialCapital, changeInMoney, Portfolio.InvestedCapital, Portfolio.MaxRiskEquityPerPosPercent, (sumCRTDR == 0.0 ? 1.0 / (double)numberOfEntries : ((1.0 - crtdr) / sumCRTDR)), cashToInvest, strategy.EntryContracts, possibleEntryPrice);
	                //strategy.EntryContracts = (int)((double)(InitialCapital + changeInMoney + (DoReinvestment ? Portfolio.NetProfit : 0.0) - Portfolio.InvestedCapital) / 100.0 * Portfolio.MaxRiskEquityPerPosPercent / numberOfEntries / Math.Abs(strategy.Signals[0].Bars.CloseValue));
					if(strategy.EntryContracts == 0)
						Log(Bars.TimeValue.ToString() + " " + strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
	            }
				
				Log("Distributed money: {0}$", sumInvested);
			}
        }
    }
}