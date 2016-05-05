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
            MaxNumberOfEntriesPerDayShortLimit = 5;
        }

        protected override void Create()
        {
			//cutlersRSIIndicatorMath = new CutlersRSIIndicatorMath(this);
			m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
            if (Environment.ApplicationCode != EApplicationCode.Portfolio)
                ExecControl.Abort("Signal can be applied in portfolio only.");
            m_KRatio.NumberOfDataBarsInCalendarYear = 1;
        }

		[Input]
		public bool DoReinvestment { get; set; }
		
        protected override void CalcBar()
        {
			//if(true) //Environment.Optimizing
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

	            // let's make the most out of our available cash
	            foreach (var strategy in PortfolioStrategies)
	            {
					var potentialEntryPrice = strategy.PortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] as double?;
	                if (potentialEntryPrice != null)
	                {
	                    listOfStrategiesWithOrders.Add(strategy);
	                    numberOfEntries++;
	                }
	            }

	 			//var rsi = cutlersRSIIndicatorMath.CalcNextValue(numberOfEntries);
				
	            // risk management says: Do not buy in case of two many entry signals, markets seem to be bumpy
                var allowShort = numberOfEntries < MaxNumberOfEntriesPerDayShortLimit;
                var allowLong = numberOfEntries < MaxNumberOfEntriesPerDayLongLimit;
	            
				foreach (var strategy in listOfStrategiesWithOrders)
	            {
					strategy.AllowEntriesShort = allowShort;
					strategy.AllowEntriesLong = allowLong;
	                strategy.EntryContracts = (int)((double)(this.InitialCapital + (DoReinvestment ? this.Portfolio.NetProfit : 0.0) - this.Portfolio.InvestedCapital) / (double)numberOfEntries / Math.Abs(strategy.PortfolioData[PortfolioHelpFunctions.PotentialEntryPrice].safe_cast2double()));
//					if(strategy.EntryContracts == 0)
//						Output.WriteLine(Bars.TimeValue.ToString() + " " + strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
	            }
			}
        }
    }
}