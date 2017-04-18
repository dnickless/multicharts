using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy {
    public class CRTDR_Signal : PortfolioSignalObject {
        public CRTDR_Signal(object _ctx):base(_ctx){}
        private IOrderMarket buyOrder;
        private IOrderMarket sellOrder;
//        private IOrderMarket sellShortOrder;
//        private IOrderMarket buyToCoverOrder;
        
//        [Input]
//        public int RsiUpMinimumForShort {get; set;}
        
        [Input]
        public int EmaLong {get; set;}
        
        [Input]
        public int EmaShort  {get; set;}
        
        [Input]
        public int RsiLengthDown {get; set;}
        
        [Input]
        public int RsiLengthFlat {get; set;}
        
        [Input]
        public int RsiLengthUp {get; set;}

        [Input]
        public double LongLimitUp  {get; set;}
        
        [Input]
        public double LongLimitDown  {get; set;}
        
        [Input]
        public double LongLimitFlat {get; set;}
        
//        [Input]
//        public double ShortLimitDown {get; set;}
        
        [Input]
        public double StopLossLevel {get; set;}
        
        [Input]
        public double RsiSellLevelUp {get; set;}
        
        [Input]
        public double RsiSellLevelDown {get; set;}
        
        [Input]
        public double RsiSellLevelFlat {get; set;}
        
//        [Input]
//        public double RsiCoverLevelUp {get; set;}
        
//        [Input]
//        public double RsiCoverLevelDown {get; set;}
        
//        [Input]
 //       public double RsiCoverLevelFlat {get; set;}
        
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathDown;
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathFlat;
		private CutlersRSIIndicatorMath cutlersRSIIndicatorMathUp;
        private XAverageThatWorks xAverageLong;
        private XAverageThatWorks xAverageShort;
		private bool doReinvestment = false;
        
        protected override void Create() {
            buyOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
            sellOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI LE", EOrderAction.Sell, OrderExit.FromAll));
            //sellShortOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));
            //buyToCoverOrder = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "RSI SE", EOrderAction.BuyToCover, OrderExit.FromAll));
            cutlersRSIIndicatorMathDown = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathFlat = new CutlersRSIIndicatorMath(this, 1);
            cutlersRSIIndicatorMathUp = new CutlersRSIIndicatorMath(this, 1);
            xAverageLong = new XAverageThatWorks(this, 1);
            xAverageShort = new XAverageThatWorks(this, 1);
        }
        
        protected override void StartCalc() {
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
			MyPortfolioData["CRTDR"] = 1;
			MyPortfolioData["EXIT"] = false;
			cutlersRSIIndicatorMathDown.Length = RsiLengthDown;
			cutlersRSIIndicatorMathFlat.Length = RsiLengthFlat;
			cutlersRSIIndicatorMathUp.Length = RsiLengthUp;
            xAverageLong.Price = Bars.Close;
			xAverageLong.Length = EmaLong;
            xAverageShort.Price = Bars.Close;
			xAverageShort.Length = EmaShort;
        }
		
		protected override void StopCalc()
		{
			if(!Environment.Optimizing)
			{
				Output.WriteLine("Dumping log for {0}", Bars.Info.Name);
				DumpLog();
			}
		}
        
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
				var str = string.Format("{0} - {1} - (AutoTrading: {2}, RealTime: {3}) - {4}: {5}", DateTime.Now, Bars.TimeValue, Environment.IsAutoTradingMode ? "YES" : "NO", Environment.IsRealTimeCalc ? "YES" : "NO", Bars.Info.Name, string.Format(message, parameters));
				
				if(Environment.IsRealTimeCalc)
				{
					logStringBuilder.AppendLine(str);
					//if(logStringBuilder.Length >= 1000)
					{
						DumpLog();
					}
				}
				
				if(sendMail)
				{
					System.Threading.ThreadPool.QueueUserWorkItem(o => Mail(str));
				}
				
				Output.WriteLine(str);
			}
		}
        
		private void DumpLog()
		{
			File.AppendAllText(@"c:\temp\logs\" + Bars.Info.Name + ".txt", logStringBuilder.ToString()); 
			logStringBuilder.Length = 0;
		}
		
        protected override void CalcBar(){
			Log("Calc Reason: {0}, Bar Status: {1}", Environment.CalcReason, Bars.Status);
			Log("Open: {0}, High: {1}, Low: {2}, Close: {3}", Bars.OpenValue, Bars.HighValue, Bars.LowValue, Bars.CloseValue);
			
			// delay buys by ten seconds
			var marketPosition = StrategyInfo.MarketPosition;
			if(Environment.CalcReason != CalculationReason.Timer)
			{
				// prevent execution in case of unexpected events
				if(Bars.Status != EBarState.Close) return;
				
//				if(Environment.IsRealTimeCalc && marketPosition == 0)
//				{
//					Log("Delaying symbol since no open position found. Let us process the sells first...");
//					ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(15));
//					return;
//				}
			}
			
            var rsiDown = cutlersRSIIndicatorMathDown.CalcNextValue(Bars.CloseValue);
            var rsiFlat = cutlersRSIIndicatorMathFlat.CalcNextValue(Bars.CloseValue);
            var rsiUp = cutlersRSIIndicatorMathUp.CalcNextValue(Bars.CloseValue);
            
            var high = Bars.HighValue;
            var low = Bars.LowValue;
            var close = Bars.CloseValue;
            
            var crtdr = CRTDRIndicatorMath.CalcNextValue(high, low, close);
			
			var xAverageLongValue = xAverageLong.Value;
			var xAverageShortValue = xAverageShort.Value;
			
			var trend = GetTrend(close, xAverageLongValue, xAverageShortValue);
			var rsi = trend == Trend.Down ? rsiDown : (trend == Trend.Up ? rsiUp : rsiFlat);
			
       		Log("Calculating strategy, high {0} low {1} close {2} rsi {3} crtdr {4} xAverageLong {5} xAverageShort {6}", high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue);
			MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = null;
			MyPortfolioData["CRTDR"] = crtdr;
			MyPortfolioData["EXIT"] = false;
			
			if(marketPosition == 0)
			{
	            if (GetSignalUp(crtdr, rsi, trend))
	            {
					var numLots = Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / Bars.CloseValue);
					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = Bars.CloseValue;
					LogAndMail("!!!LONG !!! Attempting to buy {0} shares @ {1}$", numLots, Bars.CloseValue);
	                buyOrder.Send(numLots);
	            }
//				else if (GetSignalDown(crtdr, rsi, trend))
//	            {
//	           		Output.WriteLine("SHORT on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue);
//					MyPortfolioData[PortfolioHelpFunctions.PotentialEntryPrice] = -Bars.CloseValue;
//					sellShortOrder.Send(Convert.ToInt32((InitialCapital + (doReinvestment ? Portfolio.NetProfit : 0.0)) / 10.0 / Bars.CloseValue));
//	            }
				else
				{
	           		//Log("---  FLAT - NOP ---");
				}
			}
			else if(marketPosition > 0)
			{
				// WE ARE LONG
				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(marketPosition) * StopLossLevel);
				
				// close non-profitable positions straight away because we have picked a loser and need to free up money for new deals
				if(CurrentPosition.OpenProfit < 0)
				{
	           		LogAndMail("!!!Loser cut!!! This appears to be a loser. Assumed loss: {0}$", CurrentPosition.OpenProfit);
					sellOrder.Send("Loser cut");
					//GenerateExitOnClose();
					MyPortfolioData["EXIT"] = true;
				}
				else if (this.BarsSinceEntry() > 2 && !GetSignalUp(crtdr, rsi, trend))
				{
	           		LogAndMail("!!!SELL condition 1!!! Bars since entry > 2 and no up signal, so let's exit. Assuming to cash in {0}$", CurrentPosition.OpenProfit);
					sellOrder.Send("SELL1");
					MyPortfolioData["EXIT"] = true;
				}
				else if(rsi > (trend == Trend.Down ? RsiSellLevelDown : (trend == Trend.Up ? RsiSellLevelUp : RsiSellLevelFlat)))
				{
	           		LogAndMail("!!!SELL condition 2!!! RSI condition satisfied, we take the profit (assumed to be around {0}) and run!", CurrentPosition.OpenProfit);
					sellOrder.Send("SELL2");
					MyPortfolioData["EXIT"] = true;
				}
				else
				{
	           		//Log("---  LONG - NOP ---");
				}
			}
//			else if(marketPosition < 0)
//			{
//				// WE ARE SHORT
//				GenerateStopLoss(StrategyInfo.AvgEntryPrice * Math.Abs(marketPosition) * StopLossLevel);
//				
//				if(this.BarsSinceEntry() > 2 && !GetSignalDown(crtdr, rsi, trend))
//				{
//	           		//Output.WriteLine("{8}: COVER1 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue, Bars.Info.Name);
//					buyToCoverOrder.Send();
//				}
//				else if(rsi < (trend == Trend.Down ? RsiCoverLevelDown : (trend == Trend.Up ? RsiCoverLevelUp : RsiCoverLevelFlat)))
//				{
//	           		//Output.WriteLine("{8}: COVER2 on {0}, high {1} low {2} close {3} rsi {4} crtdr {5} xAverageLong {6} xAverageShort {7}", Bars.TimeValue, high, low, close, rsi, crtdr, xAverageLongValue, xAverageShortValue, Bars.Info.Name);
//					buyToCoverOrder.Send();
//				}
//				else
//				{
//	           		Log("--- SHORT - NOP ---");
//				}
//			}
        }
        
		private enum Trend
		{
			Up = 1,
			Down = -1,
			Flat = 0
		}
		
		private Trend GetTrend(double close, double xAverageLongValue, double xAverageShortValue)
		{
            if(close > xAverageLongValue && xAverageLongValue < xAverageShortValue)
            {
				return Trend.Up;
            }
            else if (xAverageLongValue > xAverageShortValue && xAverageShortValue > close)
            {
				return Trend.Down;
            }
			else return Trend.Flat;
		}
		
        private bool GetSignalUp(double crtdr, double rsi, Trend trend)
        {
			if(trend == Trend.Up)
			{
				if(crtdr * 100 + rsi <= LongLimitUp) return true;
			}
			else if(trend == Trend.Down)
			{
				if(crtdr * 100 + rsi <= LongLimitDown) return true;
			}
			else
			{
				if(crtdr * 100 + rsi <= LongLimitFlat) return true;
			}
			
			return false;
		}
		
//		private double GetMarketPosition()
//		{
//			return this.Environment.IsAutoTradingMode ? StrategyInfo.MarketPositionAtBrokerForTheStrategy : StrategyInfo.MarketPosition;
//		}
		
//        private bool GetSignalDown(double crtdr, double rsi, Trend trend)
//        {
//			if(rsi >= RsiUpMinimumForShort && trend == Trend.Down)
//			{
//				if(crtdr * 100 + rsi >= 200 - ShortLimitDown) return true;
//			}
//            
//			return false;
//		}
		
		protected override void OnBrokerStategyOrderFilled(bool isBuy, int quantity, double avgFillPrice)
		{
			LogAndMail("{0} order filled. Quantity: {1}, avg. fill price: {2}. MarketPositionAtBroker={3}, AvgEntryPriceAtBroker={4}, MarketPositionAtBrokerForTheStrategy={5}, AvgEntryPriceAtBrokerForTheStrategy={6}, MarketPosition={7}, AvgEntryPrice={8}", isBuy ? "BUY" : "SELL", quantity, avgFillPrice, StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker, StrategyInfo.MarketPositionAtBrokerForTheStrategy, StrategyInfo.AvgEntryPriceAtBrokerForTheStrategy, StrategyInfo.MarketPosition, StrategyInfo.AvgEntryPrice);
		}
		
		protected override void OnBrokerPositionChange()
		{
			LogAndMail("MarketPositionAtBroker={0}, AvgEntryPriceAtBroker={1}, MarketPositionAtBrokerForTheStrategy={2}, AvgEntryPriceAtBrokerForTheStrategy={3}, MarketPosition={4}, AvgEntryPrice={5}", StrategyInfo.MarketPositionAtBroker, StrategyInfo.AvgEntryPriceAtBroker, StrategyInfo.MarketPositionAtBrokerForTheStrategy, StrategyInfo.AvgEntryPriceAtBrokerForTheStrategy, StrategyInfo.MarketPosition, StrategyInfo.AvgEntryPrice);
		}
		
        // Recalculate the last bar when performing the recalculation
        protected override void OnRecalcLastBarAfterEvent()
        {
//			Log("ReCalc event");
//            CalcBar();
        }
		
		private static string fromPassword = System.IO.File.ReadAllText(@"C:\temp\do_not_delete\asdf.txt");
		private static MailAddress fromAddress = new MailAddress("ramanddan.trading@gmail.com", "RamAndDan Trading");
		
		private void Mail(string message)
		{
			if(Environment.IsRealTimeCalc)
			{
				try
				{
					//Log("Sending email");
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
						Subject = "Hello from the other side...",
						Body = message
					})
					{
						mailMessage.To.Add("ramon@winter-berg.com");
						mailMessage.To.Add("daniel.hegener@gmx.net");
						smtp.Send(mailMessage);
					}
				}
				catch(Exception)
				{
				}
			}
		}
 	}
}