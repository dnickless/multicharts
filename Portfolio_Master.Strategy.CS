using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
	public static class PortfolioDataKeys
	{
		public const string CRTDR = "CRTDR";
		public const string MoneyManagementCommand = "MoneyManagementCommand";
		public const string IWantToBuy = "IWantToBuy";
		public const string IWantToSell = "IWantToSell";
		public const string NumberOfShares = "NumberOfShares";
	}
	
	public enum StrategyEvents         // The list of possible messages, sent between the strategy and MM-signal
	{
		None = 0,                      // nothing
		GenerateOrders_Long,           // Order generation command
		GenerateOrders_Short,          // Sell generation command
	}
	
    public class Portfolio_Master : PortfolioSignalObject
    {
		[Input]
        public double SkalpingFactor { get; set; }
		
		[Input]
		public string TriggerTime { get; set; }
		TimeSpan triggerTime;
		
		private K_RatioMath m_KRatio;
		
	    private DateTime m_LastTraceTime = DateTime.MinValue;

		public Portfolio_Master(object _ctx)
            : base(_ctx)
        {
			SkalpingFactor = 1.1;
        }

        protected override void Create()
        {
			m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
			triggerTime = TimeSpan.Parse(TriggerTime);
			
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
				DumpLog(false);
			}
		}

		[Input]
		public bool DoReinvestment { get; set; }
		
		private StringBuilder logStringBuilder = new StringBuilder();
		
		private void Log(string message, params object[] parameters)
		{
			Log(false, message, parameters);
		}
		
		private void LogAndMail(string message, params object[] parameters)
		{
			Log(true, message, parameters);
		}
		
		private void Log(bool sendMail, string message, params object[] parameters)
		{
			if(!Environment.Optimizing)
			{
				var str = string.Format("{0} - {1} - (AutoTrading: {2}, RealTime: {3}) - {4}: {5}", DateTime.Now, Bars.TimeValue, Environment.IsAutoTradingMode ? "YES" : "NO", Environment.IsRealTimeCalc ? "YES" : "NO", "MONEY MGMT", string.Format(message, parameters));
				
				if(Environment.IsRealTimeCalc)
				{
					logStringBuilder.AppendLine(str);
					if(sendMail)
					{
						DumpLog(true);
					}
//					else if(logStringBuilder.Length >= 1000)
//					{
//						DumpLog(false);
//					}
				}
				
				//Output.WriteLine(str);
			}
		}
		
		private void DumpLog(bool sendMail)
		{
			string str = logStringBuilder.ToString();
			File.AppendAllText(@"c:\temp\logs\_MoneyManagement.txt", str); 
			if(sendMail)
			{
				System.Threading.ThreadPool.QueueUserWorkItem(o => Mail(str));
			}
			logStringBuilder.Length = 0;
		}
		
		/*
		* This method returns current bar close time or session close, depending on the resolution
		*/
		protected TimeSpan GetCloseTime()         
		{
			switch(Bars.Request.Resolution.Type)
			{
				case EResolution.Day:
					foreach (SessionObject s in Bars.Sessions)
					{
						if (s.EndDay == DateTime.Now.DayOfWeek)
						if (s.EndOfSession)
						return s.EndTime;
					}
					break;

				case EResolution.Second:
				case EResolution.Minute:
				case EResolution.Hour:
					return Bars.TimeValue.TimeOfDay;
			}
			throw new Exception("Can't calculate close time.");  // If the resolution is not time-based, it is not possible to determine bar close time.
		}

		DateTime lastClosedDate;
		
        protected override void CalcBar()
        {
		    if (!Bars.LastBarOnChart) return;                                                                    // Ignore BackTesting
			
			//Log("Calculating");
			
      		//var time = GetCloseTime() - TimeSpan.FromSeconds(30);                                                 // and calculate the moment when the calculated coefficient will be requested from each trading strategy.
                                                                                                                    // In this case - 30 seconds before the bar close
      		var time = triggerTime;
			int strategiesCount = PortfolioStrategies.Count;
			
			if (Bars.Status == EBarState.Close)                                                                   // When the bar closes, it is necessary to clear the collection of coefficients
			{                                                                                                     // it will be a flag: if the collection is empty, then either the calculation has just started, or
				                                                                                                  // the new bar has appeared and we need to get new coefficients.
				LogAndMail("Close event received.");
				lastClosedDate = Bars.TimeValue.Date;
				foreach (var strategy in PortfolioStrategies)
				{
					strategy.Status = "Waiting for MMS command...";
				}
				return;                                                                                           // The request of new values of coefficients will be executed on the new bar already.
			}
			
			TimeSpan currentTime = DateTime.Now.TimeOfDay;
			// If it is already necessary to request values from signals, we start the process of receiving values unless the close signal has been received before
			if (currentTime < time || lastClosedDate == Bars.TimeValue.Date || currentTime >= GetCloseTime())
			{
				return;
			}
			
			var listOfStrategiesWithBuyOrders = new List<IPortfolioStrategy>();

			double sumCRTDR = 0.0;
			var crtdrs = new List<double>();

			double changeInMoney = 0;
			
			foreach (var strategy in PortfolioStrategies)
			{
	            // let's make the most out of our available cash
				// loop through all symbols to calculate sum of all CRTDR values for pro-rata cash distribution
				var crtdr = (double)(strategy.PortfolioData[PortfolioDataKeys.CRTDR] ?? 1.0);
				var wantsToBuy = (bool)(strategy.PortfolioData[PortfolioDataKeys.IWantToBuy] ?? false);
                if (wantsToBuy)
                {
                    listOfStrategiesWithBuyOrders.Add(strategy);
					crtdrs.Add(crtdr);
					sumCRTDR += (1.0 - crtdr);
                }
				var wantsToSell = (bool)(strategy.PortfolioData[PortfolioDataKeys.IWantToSell] ?? false);
				if(wantsToSell)
				{
					strategy.PortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.GenerateOrders_Short;
					changeInMoney += strategy.Signals[0].CurrentPosition.OpenTrades.Sum(t => t.EntryOrder.Price * t.EntryOrder.Contracts);
				}
				
				strategy.Status = string.Format("CRTDR: {0:0.00}, {1}", crtdr, wantsToBuy ? "BUY" : (wantsToSell ? "SELL" : "NOP"));
			}

			// let's only trade the triggering symbols that are "above" average in terms of the CRTDR criteria
			if(listOfStrategiesWithBuyOrders.Count != 0)
			{
				var average = crtdrs.Average();
				var sumOfSquaresOfDifferences = crtdrs.Sum(val => (val - average) * (val - average));
				var stdDev = Math.Sqrt(sumOfSquaresOfDifferences / crtdrs.Count);
				
				var meanCRTDR = sumCRTDR / (double)listOfStrategiesWithBuyOrders.Count;
		        foreach (var strategy in listOfStrategiesWithBuyOrders.ToList() /* clone list because we're modifying it below */)
		        {
					bool wantsToBuy = (bool)strategy.PortfolioData[PortfolioDataKeys.IWantToBuy];
	                if (wantsToBuy)
					{
			            var crtdr = (double)strategy.PortfolioData[PortfolioDataKeys.CRTDR];
						var cutOffCrtdr = meanCRTDR + stdDev * SkalpingFactor;
						if(1.0 - crtdr < cutOffCrtdr)
			            {
							// adjust previously calculated values
			                listOfStrategiesWithBuyOrders.Remove(strategy);
							Log("Removing because of CRTDR value {0} which is below the calculated cut-off value of {1}", crtdr, cutOffCrtdr);
			                sumCRTDR -= (1.0 - crtdr);
			            }
					}
		        }
			}
			
			double sumInvested = 0.0;
			foreach (var strategy in listOfStrategiesWithBuyOrders)
            {
				var crtdr = (double)strategy.PortfolioData[PortfolioDataKeys.CRTDR];
				double availableCash = 0.0;
				if(DoReinvestment)
				{
					availableCash = Math.Max(0, InitialCapital + changeInMoney + Portfolio.NetProfit - Portfolio.InvestedCapital);
				}
				else
				{
					// do not reinvest potential profits, cap @ InitialCapital
					availableCash = Math.Max(0, Math.Min(InitialCapital, InitialCapital + changeInMoney - Portfolio.InvestedCapital));
				}
				
				var cashToInvest = availableCash / 100.0 * Portfolio.MaxRiskEquityPerPosPercent * (sumCRTDR == 0.0 ? 1.0 / (double)listOfStrategiesWithBuyOrders.Count : ((1.0 - crtdr) / sumCRTDR));
				sumInvested += cashToInvest;
				var possibleEntryPrice = Math.Abs(strategy.Signals[0].Bars.CloseValue);
                strategy.EntryContracts = (int)(cashToInvest / possibleEntryPrice);
				Log("({0}: {1} - {2}) / 100.0 * {3} * {4} = {5}$ --> {6}# @ assumed price {7}$ --- available cash: {8}, SUM(CRTDR): {9}", strategy.Signals[0].Bars.Info.Name, InitialCapital, Portfolio.InvestedCapital, Portfolio.MaxRiskEquityPerPosPercent, (sumCRTDR == 0.0 ? 1.0 / (double)listOfStrategiesWithBuyOrders.Count : ((1.0 - crtdr) / sumCRTDR)), cashToInvest, strategy.EntryContracts, possibleEntryPrice, availableCash, sumCRTDR);
				if(strategy.EntryContracts > 0)
				{
					strategy.PortfolioData[PortfolioDataKeys.NumberOfShares] = strategy.EntryContracts;
					strategy.PortfolioData[PortfolioDataKeys.MoneyManagementCommand] = StrategyEvents.GenerateOrders_Long;
				}
				else
				{
					Log(Bars.TimeValue.ToString() + " " + strategy.Signals[0].Bars.Info.Name + " OUT OF MONEY, CANNOT BUY.");
				}
            }
			
			Log("Distributed money: {0}$", sumInvested);
//			if(Environment.Optimizing)
//			{
//				var result = m_KRatio.Value;
//				if (Bars.LastBarOnChart)
//				{
//	            	CustomFitnessValue = result;
//				}
//			}
		}
		
		private static string fromPassword = System.IO.File.ReadAllText(@"C:\temp\do_not_delete\asdf.txt");
		private static MailAddress fromAddress = new MailAddress("ramanddan.trading@gmail.com", "RamAndDan Trading");
		
		private void Mail(string message)
		{
			if(Environment.IsRealTimeCalc)
			{
				try
				{
					using(SmtpClient smtp = new SmtpClient
					{
						Host = "smtp.gmail.com",
						Port = 587,
						EnableSsl = true,
						DeliveryMethod = SmtpDeliveryMethod.Network,
						UseDefaultCredentials = false,
						Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
					})
					using (var mailMessage = new MailMessage()
					{
						From = fromAddress,
						Subject = "Trading day summary: Money Management Signal",
						Body = message
					})
					{
						mailMessage.To.Add("ramon@winter-berg.com");
						mailMessage.To.Add("daniel.hegener@gmx.net");
						smtp.Send(mailMessage);
					}
				}
				catch(Exception e)
				{
					Log("Error sending email: {0}", e.Message);
				}
			}
		}
	}
}