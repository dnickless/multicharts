using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;
using System.Collections.Generic;


/*  
 * RSIRankEx_SignalBase
 *  This signal is calculated on each symbol, RSI function value for each instrument is calculated. Then, this value is sent to MM-signal upon request.
 *  According to MM-signal command, this signal can generate short entry, long entry no order.
 * 
 */



namespace PowerLanguage.Strategy
{
  public class RSIRankEx_SignalBase : PortfolioSignalObject
  {
    public RSIRankEx_SignalBase(object _ctx)
      : base(_ctx)
    {
      RSI_Length = 14;    // Period length of RSI function is set in the input. It is 14 bars by default. This function will calculate the evaluating coefficient.
    }

    [Input]
    public int RSI_Length { get; set; }

    private RSI m_RSI;


    private IOrderMarket buy_order, sell_order;
    protected override void Create()
    {
      // In this method we create orders and functions, required for further signal functioning
      buy_order = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "Long", EOrderAction.Buy));
      sell_order = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, "Short", EOrderAction.SellShort));

      m_RSI = new RSI(this);
    }
    protected override void StartCalc()
    {
      // In this method it is specified that the signal is calculated in Portfolio Trader.
      if (Environment.ApplicationCode != EApplicationCode.Portfolio)
        ExecControl.Abort("The signal can be used in MultiCharts Portfolio Trader only.");

      // and initialize the function
      m_RSI.price = Bars.Close;
      m_RSI.length = RSI_Length;
    }

    protected override void CalcBar()
    {
      double Rating = m_RSI[0];  // Get RSI function value for the current bar.

      if (!Environment.IsRealTimeCalc)    // There is no need to calculate on historical data, that is why backtesting is ignored
      {
        // As soon as backtesting is finished, the signal gets immediately calculated by the timer.
        // It is necessary to calculate RSI function value, which can be requested by MM-signal at any moment.
        ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(5));
        return;
      }

      StaretegyEvents cmd = StaretegyEvents.None;
      object obj = MyPortfolioData["MM_Command"];   // Check, if there are any MM-signal events available

      if (!object.Equals(obj, null))
        cmd = (StaretegyEvents)obj;


      Output.WriteLine("\t\t{0}: Symbol: {1}, Command: {2}", DateTime.Now, Bars.Info.Name, cmd);

      switch (cmd) // Execute specific commands, depending on the event
      {
        case StaretegyEvents.GetFactor:                                                                       // If MM-signal requested rating coefficient from the signal, it will be sent.
          MyPortfolioData["RankingValue"] = Rating;                                                           // In this case it is RSI function value on the current bar.
          MyPortfolioData["MM_Command"] = StaretegyEvents.DataWasSent;                                         // And will inform that the data was sent.
          Output.WriteLine("\t\t{0}: Send Data for Symbol: {1}\t{2}", DateTime.Now, Bars.Info.Name, Rating);
          ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(5));                                            // Also, it is necessary to re-calculate, as the final instructions were not received yet.
          break;

        case StaretegyEvents.GenerateOrders_Long:                                                             // If MM-signal gives command to generate a long entry order
          buy_order.Send();                                                                                   // the signal will execute it here.
          Output.WriteLine("\t\t{0}: Generate Buy. {1}", DateTime.Now, Bars.Info.Name);
          break;

        case StaretegyEvents.GenerateOrders_Short:                                                            // If MM-signal gives command to generate a short entry order
          sell_order.Send();                                                                                  // the signal will execute it here.
          Output.WriteLine("\t\t{0}: Generate Short. {1}", DateTime.Now, Bars.Info.Name);
          break;

        case StaretegyEvents.None:
          ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(5));                                            // If there were no instructions from MM-signal, we will keep waiting for them.
          break;
      }

      if (Bars.Status == EBarState.Close)                                                                     // During calculation at the bar close, global variables, used to exchange messages between the current signal and MM-signal
      {                                                                                                       // should be zeroed
        MyPortfolioData["RankingValue"] = 0;                                                                  
        MyPortfolioData["MM_Command"] = StaretegyEvents.None;
        Output.WriteLine("\t\tSeries {0} is Closed with Ranking Value: {1}. Status: {2}, cmd: {3}", Bars.Info.Name, Rating, Bars.Status, cmd);
        ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(5));                                              // And start the timer to wait for the new commands from MM-signal
      }
    }

    protected override void OnRecalcLastBarAfterEvent()
    {
      CalcBar();    // Call CalcBar() method within each tick of the timer
    }
  }
}