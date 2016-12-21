using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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

		[Input]
        public double SkalpingFactor { get; set; }

		//CutlersRSIIndicatorMath cutlersRSIIndicatorMath;
		private K_RatioMath m_KRatio;
		
        public Portfolio_Master(object _ctx)
            : base(_ctx)
        {
            MaxNumberOfEntriesPerDayShortLimit = 2;
            MaxNumberOfEntriesPerDayLongLimit = 5;
			SkalpingFactor = 1.1;
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
			
            m_KRatio.NumberOfDataBarsInCalendarYear = 1; //252;
        }
		
		protected override void StopCalc()
		{
			if(!Environment.Optimizing)
			{
				Output.WriteLine("Dumping Money Management Signal log");
				DumpLog();
			}
		}

		[Input]
		public bool DoReinvestment { get; set; }
		
		private StringBuilder logStringBuilder = new StringBuilder();
		
		private void Log(string message, params object[] parameters)
		{
			if(!Environment.Optimizing)
			{
				var str = string.Format("{0} - {1} - ({2}) - {3}: {4}", DateTime.Now, Bars.TimeValue, Environment.IsRealTimeCalc ? "REALTIME" : "HISTORIC", "MONEY MGMT", string.Format(message, parameters));
				
				if(Environment.IsRealTimeCalc)
				{
					logStringBuilder.AppendLine(str);
					if(logStringBuilder.Length >= 1000000)
					{
						DumpLog();
					}
				}
				
				Output.WriteLine(str);
			}
		}
		
		private void DumpLog()
		{
			File.AppendAllText(@"c:\temp\logs\_MoneyManagement.txt", logStringBuilder.ToString()); 
			logStringBuilder.Length = 0;
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
				var crtdrs = new List<double>();
				
	            // let's make the most out of our available cash
				// loop through all symbols to calculate sum of all CRTDR values for pro-rata cash distribution
	            foreach (var strategy in PortfolioStrategies)
	            {
	                if ((strategy.PortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] as double?) != null)
	                {
	                    listOfStrategiesWithOrders.Add(strategy);
	                    numberOfEntries++;
						var crtdr = (double)strategy.PortfolioData["CRTDR"];
						crtdrs.Add(crtdr);
						sumCRTDR += (1.0 - crtdr);
	                }
					
					var exit = strategy.PortfolioData["EXIT"] as bool?;
					if(exit.HasValue && exit.Value)
					{
						changeInMoney += strategy.Signals[0].CurrentPosition.OpenTrades.Sum(t => t.EntryOrder.Price * t.EntryOrder.Contracts);
					}
	            }
				
				Log("Money likely to come in due to sells: {0}", changeInMoney);
				
				// let's only trade the triggering symbols that are "above" average in terms of the CRTDR criteria
				if(numberOfEntries != 0)
				{
					var average = crtdrs.Average();
					var sumOfSquaresOfDifferences = crtdrs.Sum(val => (val - average) * (val - average));
					var stdDev = Math.Sqrt(sumOfSquaresOfDifferences / crtdrs.Count);
					
					var meanCRTDR = sumCRTDR / (double)numberOfEntries;
			        foreach (var strategy in PortfolioStrategies)
			        {
		                if ((strategy.PortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] as double?) != null)
						{
				            var crtdr = (double)strategy.PortfolioData["CRTDR"];
							if(1.0 - crtdr < meanCRTDR + stdDev * SkalpingFactor)
				            {
								strategy.AllowEntriesLong = false;
								// adjust previously calculated values
				                listOfStrategiesWithOrders.Remove(strategy);
								Log("Removing {0}", crtdr);
								//strategy.EntryContracts = 0;
				                numberOfEntries--;
				                sumCRTDR -= (1.0 - crtdr);
				            }
						}
			        }
				}

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
					Log("Buying {0}", crtdr);
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
	                //strategy.EntryContracts = (int)((double)(InitialCapital + changeInMoney + (DoReinvestment ? Portfolio.NetProfit : 0.0) - Portfolio.InvestedCapital) / 100.0 * Portfolio.MaxRiskEquityPerPosPercent / numberOfEntries / Math.Abs(strategy.Signals[0].Bars.CloseValue));
					Log("({0}: {1} + {2} - {3}) / 100.0 * {4} * {5} = {6}$ --> {7}# @ assumed price {8}$ --- available cash: {9}", strategy.Signals[0].Bars.Info.Name, InitialCapital, changeInMoney, Portfolio.InvestedCapital, Portfolio.MaxRiskEquityPerPosPercent, (sumCRTDR == 0.0 ? 1.0 / (double)numberOfEntries : ((1.0 - crtdr) / sumCRTDR)), cashToInvest, strategy.EntryContracts, possibleEntryPrice, availableCash);
					if(strategy.EntryContracts == 0)
						Log(Bars.TimeValue.ToString() + " " + strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
	            }
				
				Log("Distributed money: {0}$", sumInvested);
			}
        }
   }
}