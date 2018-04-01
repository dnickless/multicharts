using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;
using ATCenterProxy.interop;
using ATCenterProxy;
using PowerLanguage.TradeManager;

namespace PowerLanguage.Strategy
{
  public class Test_GetAccountInfo : SignalObject
  {
    public Test_GetAccountInfo( object _ctx ) : base( _ctx ) { }
    bool isSubscribet;

    protected override void Create()
    {
      isSubscribet = false;
    }

    void Accounts_Added( params TradeManager.Account[] _items )
    {
      foreach (TradeManager.Account x in _items)
      {
        Output.WriteLine( "Profile Name: {0}, Balance: {1}", x.Profile, x.Balance );
      }
    }

    protected override void StartCalc()
    {
      if (!isSubscribet)
      {
        TradeManager.TradingData.Accounts.Added +=
          new PowerLanguage.TradeManager.TItemsChanged<PowerLanguage.TradeManager.Account>( Accounts_Added );

        TradeManager.TradingData.Accounts.Changed +=
          new PowerLanguage.TradeManager.TItemsChanged<PowerLanguage.TradeManager.Account>( Accounts_Changed );

        isSubscribet = true;
      }
    }

    void Accounts_Changed( params TradeManager.Account[] _items )
    {
      foreach (TradeManager.Account x in _items)
      {
        Output.WriteLine( "Profile Name: {0}, Balanse: {1}", x.Profile, x.Balance );
      }
    }

    protected override void CalcBar()
    {
      TradeManager.ProcessEvents();
    }
  }
}