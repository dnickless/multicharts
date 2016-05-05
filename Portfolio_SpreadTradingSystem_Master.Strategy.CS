using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
  public class Portfolio_SpreadTradingSystem_Master : PortfolioSignalObject
  {
    public static string MasterMaxPosAgo = "MasterMaxpositionsago";
    public static string MasterMoney = "MPMoney";

    [Input]
    public int Length { get; set; }
    [Input]
    public int PercentEquity { get; set; }

    public Portfolio_SpreadTradingSystem_Master( object _ctx )
      : base(_ctx)
    {
      Length = 10;
      PercentEquity = 10;
    }

    private VariableSeries<Double> Ratio;
    private XAverage m_expAvgRatio;

    private double m_expAvgRatioVal;
    private double m_stdDevRatioVal;

    private double m_cur_pos;

    private IOrderMarket m_long;
    private IOrderMarket m_short;
    private IOrderMarket m_lx;
    private IOrderMarket m_sx;

    protected override void Create()
    {
      Ratio = new VariableSeries<double>(this);
      m_expAvgRatio = new XAverage(this);

      m_long = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.Buy));
      m_short = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.UserSpecified, EOrderAction.SellShort));

      m_lx = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.Sell));
      m_sx = OrderCreator.MarketThisBar(new SOrderParameters(Contracts.Default, EOrderAction.BuyToCover));
    }
    protected override void StartCalc()
    {
      m_expAvgRatio.Price = Ratio;
      m_expAvgRatio.Length = Length;
    }
    protected override void CalcBar()
    {
      Ratio.Value = Bars.CloseValue / BarsOfData(2).CloseValue;
	  if (Bars.CurrentBar < Length) return;
      double contracts = this.PortfolioEquity() * PercentEquity / 100;

      if (Bars.CurrentBar > 1)
      {
        if (PublicFunctions.DoubleLess(m_expAvgRatioVal + m_stdDevRatioVal, Ratio.Value))
        {
          if (!PublicFunctions.DoubleEquals(m_cur_pos, -1))
          {
            m_cur_pos = -1;
            m_short.Send((int)contracts);
          }
        }
        else
          if (PublicFunctions.DoubleGreater(m_expAvgRatioVal - m_stdDevRatioVal, Ratio.Value))
          {
            if (!PublicFunctions.DoubleEquals(m_cur_pos, 1))
            {
              m_cur_pos = 1;
              m_long.Send((int)contracts);
            }
          }
          else
          {
            m_cur_pos = 0;
            m_lx.Send();
            m_sx.Send();
          }                                       
      }

      m_expAvgRatioVal = m_expAvgRatio.Value;
      m_stdDevRatioVal = Ratio.StandardDeviationCustom(Length, 1);

      if (Environment.ApplicationCode == EApplicationCode.Portfolio)
      {
        int slaveIdx = this.GetFirstStrategyIndexBySymbolName(BarsOfData(2).Info.Name);
        if (slaveIdx < 0) ExecControl.Abort(@"specified slave trader on instrument ""{0}"" not found", BarsOfData(2).Info.Name);

        double money = Math.Abs(m_cur_pos * contracts) * Bars.CloseValue * Bars.Info.BigPointValue;

		if (PublicFunctions.DoubleGreater(money, 0))
          money = this.FromSymbolToPortfolioCurrency(money);

		PortfolioStrategies[slaveIdx].PortfolioData[MasterMoney] = -m_cur_pos * money;
      }
    }
  }
}