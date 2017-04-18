using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;
using System.Collections.Generic;
using PowerLanguage.TradeManager;

/*
 * RSIRankEx_MM - signal that receives values from a certain strategy coefficient and basing on it gives the command to send orders of the specific strategy
 * 
 */


namespace PowerLanguage.Strategy
{
  public enum StaretegyEvents      // The list of possible messages, sent between the strategy and MM-signal
  {
    None = 0,                      // nothing
    GetFactor,                     // Request to the strategy to send evaluating coefficient into MM-signal
    DataWasSent,                   // Strategy message that data was set
    GenerateOrders_Long,           // Order generation command
    GenerateOrders_Short,          // Order generation command
  }
  public class RSIRankEx_MM : PortfolioSignalObject
  {
    public RSIRankEx_MM(object _ctx) : base(_ctx)
    {
      MaxBest = 5;                                     // Number of maximum entries at the best instruments, set via the input, 5 by default
      MaxWorst = 5;                                    // Number of maximum entries at the worst instruments, set via the input, 5 by default
    }

    [Input]
    public int MaxBest { get; set; }

    [Input]
    public int MaxWorst { get; set; }

    private Dictionary<int, double> RankFromStrategies = new Dictionary<int, double>();  // Key-value collection, where "key" - strategy index, "value" - evaluating coefficient, received from the strategy
    TimeSpan m_time;

    protected override void Create()
    {
    }

    protected override void StartCalc()
    {
      if (Environment.ApplicationCode != EApplicationCode.Portfolio)                                        // Verification: the signal is only for the Portfolio Trader
        ExecControl.Abort("The signal can be used in MultiCharts Portfolio Trader only.");

      if(PortfolioStrategies.Count < MaxBest + MaxWorst)                                                    // Verification: sum of the inputs should be not more than the total number of the traded symbols
        throw new Exception("Portfolio RSIRankEx_MM Signal, please check inputs, MaxBest + MaxWorst should be less or equal to the number of tradeable instruments");

      if (Bars.Request.TimeZone != RequestTimeZone.Local)
        throw new Exception("The signal can be used only with Local time zone set.");                       // Verification: only Local time zone
    }

    protected DateTime m_LastTraceTime;
    protected override void CalcBar()
    {
      if (!Bars.LastBarOnChart) return;                                                                    // Ignore BackTesting

      TimeSpan currentTime = DateTime.Now.TimeOfDay;                                                       // Get the current time
      int strategiesCount = PortfolioStrategies.Count;

      m_time = GetCloseTime() - TimeSpan.FromSeconds(30);                                                 // and calculate the moment when the calculated coefficient will be requested from each trading strategy.
                                                                                                          // In this case - 30 seconds before the bar close
      #region Request Data
      if (Bars.Status == EBarState.Close)                                                                 // When the bar closes, it is necessary to clear the collection of coefficients
      {                                                                                                   // it will be a flag: if the collection is empty, then either the calculation has just started, or
        RankFromStrategies.Clear();                                                                       // the new bar has appeared and we need to get new coefficients.
        return;                                                                                           // The request of new values of coefficients will be executed on the new bar already.
      }

      StaretegyEvents cmd = StaretegyEvents.None;
      if (currentTime >= m_time && RankFromStrategies.Count != strategiesCount)                           // If it is already necessary to request values from signals, we start the process of receiving values
      {
        for (int idx = 0; idx < strategiesCount; idx++)                                                   // We address to each signal in cycle:
        {                                                                                                 
          PortfolioStrategies[idx].Status = "";
          object obj = PortfolioStrategies[idx].PortfolioData["MM_Command"];

          if (!object.Equals(obj, null))
            cmd = (StaretegyEvents)obj;

          if (cmd != StaretegyEvents.DataWasSent)                                                        // if it did not yet send the data
          {
            PortfolioStrategies[idx].PortfolioData["MM_Command"] = StaretegyEvents.GetFactor;             // we make a request
            Output.WriteLine("{0} MMS: Request Factor", DateTime.Now);
          }
        }
        RankFromStrategies.Clear();                                                                      // Clear the collection from the old values
      }
      #endregion

      var inLongStrategies = new List<int>();
      var inShortStrategies = new List<int>();

      #region Load Data                                                                                  
      if (RankFromStrategies.Count == 0)                                                                 // The data should be received from all the strategies at the same time
      {
        for (int idx = 0; idx < strategiesCount; idx++)
        {
          StaretegyEvents _flag = StaretegyEvents.None;
          object _o = PortfolioStrategies[idx].PortfolioData["MM_Command"];
          if (!object.Equals(_o, null))
            _flag = (StaretegyEvents)_o;

          if (_flag != StaretegyEvents.DataWasSent)                                                     // if a strategy did not yet send its data
          {
            RankFromStrategies.Clear();                                                                 // we will initiate receiving data until it is done
            {
              return;                                                                                   // otherwise, the signal will not be able to correctly evaluate the market situation
            }
          }

          double rankingValue = 0;
          rankingValue = PortfolioStrategies[idx].PortfolioData["RankingValue"].safe_cast2double();     // The type of received values should be changed to double
          PortfolioStrategies[idx].Status = String.Format("{0:0.00}", rankingValue);                    // and output them into the table, to Custom Text column

          Output.WriteLine("{1} MMS:  data received: {0}", PortfolioStrategies[idx].Signals[0].Bars.Info.Name, DateTime.Now);
          RankFromStrategies.Add(idx, rankingValue);                                                    // Put the received data into the collection
          PortfolioStrategies[idx].PortfolioData["MM_Command"] = StaretegyEvents.None;                   // Remove command for data request. It is not needed to execute it on the current bar.
        }

        RankFromStrategies = RankFromStrategies.OrderByDescending(elem => elem.Value).ToDictionary(x => x.Key, y => y.Value);     // When values of all the coefficiens are received, sort out the collection in descending order
                                                                                                                                  // symbols with maximum coefficient value will be at the top
                                                                                                                                  // symbols with minimum values will be at the bottom
      }                                                              
      #endregion

      int inShortcnt = this.StrategiesInShort(ref inShortStrategies);                 // Get the number of opened short positions
      int inLongtcnt = this.StrategiesInLong(ref inLongStrategies);                   // Get the number of opened long positions

      TimeSpan dif = DateTime.Now - m_LastTraceTime;                                  // Trace the intermediate values
      if (dif.Seconds > 1)
      {
        Output.WriteLine("{7} MMS:  {0} CalcBar. Bar Time: {1}, reqtime: {2}, strategiesToEnter.Count: {3}, strategiesCount: {4}, inShortcnt: {5}, inLongtcnt: {6}",
            currentTime, Bars.TimeValue, m_time, RankFromStrategies.Count, strategiesCount, inShortcnt, inLongtcnt, DateTime.Now);
        m_LastTraceTime = DateTime.Now;
      } 

                                                                                                                                    
      if (RankFromStrategies.Count == strategiesCount)                               // Make sure that all the coefficients were received and start sending commands to the signals
      {                                                                              
        int CountNewPositionsBest = MaxBest - inShortcnt;                            // Calculate how many additional short orders can be generated
        int CountNewPositionsWorst = MaxWorst - inLongtcnt;                          // and long orders

        for (int idx = 0; idx < Math.Min(CountNewPositionsBest, strategiesCount); idx++)   // and select the best symbols in the cycle and send the command to generate the order
        {
          if (idx >= RankFromStrategies.Count())
            break;

          int key = RankFromStrategies.Keys.ElementAt(idx);
          double value = RankFromStrategies[key];

          if (Environment.IsAutoTradingMode && Bars.LastBarOnChart)
            Output.WriteLine(DateTime.Now.ToString() + " MMS:  " + currentTime + "; idx: " + key + "; Symbol To Enter: " + PortfolioStrategies[key].Signals[0].Bars.Info.Name + "; Rank: " + value);

          if (value > 65)                                                                                 // ...if the coefficient values allow to do that
          {
            PortfolioStrategies[key].PortfolioData["MM_Command"] = StaretegyEvents.GenerateOrders_Short;  // command to send short entry order is given to the specified strategy
          }
        }

        for (int idx = strategiesCount - 1; idx > strategiesCount - Math.Min(CountNewPositionsWorst, strategiesCount) && idx > 0; idx--)    // the same for the long position
        {                                                                                                                                   // but coefficients are taken from the bottom of the list, that is, the worst
          if (idx >= RankFromStrategies.Count() || idx < 0)
            break;

          int key = RankFromStrategies.Keys.ElementAt(idx);
          double value = RankFromStrategies[key];

          if (Environment.IsAutoTradingMode && Bars.LastBarOnChart)
            Output.WriteLine(DateTime.Now.ToString() + " MMS:  " + currentTime + "; idx: " + key + "; Symbol To Enter: " + PortfolioStrategies[key].Signals[0].Bars.Info.Name + "; Rank: " + value);
          if (value < 35)
          {
            PortfolioStrategies[key].PortfolioData["MM_Command"] = StaretegyEvents.GenerateOrders_Long;
          }
        }                   
      }                     
    }

    protected TimeSpan GetCloseTime()         
    {
      /*
       * This method returns current bar close time or session close, depending on the resolution
       */
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
  }
}