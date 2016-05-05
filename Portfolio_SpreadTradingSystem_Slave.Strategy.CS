using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
  public class Portfolio_SpreadTradingSystem_Slave : PortfolioSignalObject
  {
    public Portfolio_SpreadTradingSystem_Slave( object _ctx ) : base(_ctx) { }

    private IOrderMarket buy, sellshort;
    private IOrderMarket buytocover, sell, buytocover2, sell2;

    protected override void Create()
    {
      buy = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
      sellshort = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));

      sell = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell, OrderExit.FromAll));
      buytocover = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover, OrderExit.FromAll));
      sell2 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Sell));
      buytocover2 = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.BuyToCover));
    }

    protected override void StartCalc()
    {
    }

    protected override void CalcBar()
    {
      double money = this.FromPortfolioToSymbolCurrency(
          MyPortfolioData[Portfolio_SpreadTradingSystem_Master.MasterMoney].safe_cast2double());

      double potentialEntryPrice = Bars.CloseValue;
      if (CurrentPosition.Value != 0)
        potentialEntryPrice = this.EntryPrice();

      int masterPos = (int)Math.Ceiling(money / (potentialEntryPrice * Bars.Info.BigPointValue));
      if (masterPos > 0) masterPos--;

      if (Math.Sign(masterPos) != Math.Sign(CurrentPosition.Value))
      {
        if (money == 0)
        {
          if (CurrentPosition.Side == EMarketPositionSide.Long)
            sell.Send();
          else
            buytocover.Send();
          return;
        }

        int posDifference = masterPos - CurrentPosition.Value;
        if (posDifference > 0)
        {
          if (Math.Sign(masterPos) != Math.Sign(CurrentPosition.Value))
            buy.Send(masterPos);
          else
            buytocover2.Send(posDifference);
        }
        else
        {
          if (Math.Sign(masterPos) != Math.Sign(CurrentPosition.Value))
            sellshort.Send(Math.Abs(masterPos));
          else
            sell2.Send(Math.Abs(posDifference));
        }
      }
    }
  }
}