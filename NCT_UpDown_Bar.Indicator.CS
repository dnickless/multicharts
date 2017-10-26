using System;
using System.Drawing;
using System.Linq;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
  [SameAsSymbol(true)]
  public class NCT_UpDown_Bar : IndicatorObject
  {
    public NCT_UpDown_Bar(object _ctx) 
      : base(_ctx)
    {
      UpMargin_pt = 5;
      UpColor = Color.Green;
      DnColor = Color.Red;
    }

    IPlotObject plot1;

    [Input]
    public int UpMargin_pt { get; set; }
    [Input]
    public Color UpColor { get; set; }
    [Input]
    public Color DnColor { get; set; }

    protected override void Create()
    {
      plot1 = AddPlot(new PlotAttributes("Up/Down", EPlotShapes.Point, Color.Yellow,
                                           Color.Empty, 10, 0, false));
    }

    protected override void StartCalc()
    {
    }

    protected override void CalcBar()
    {
      switch(Bars.Request.Resolution.ChartType )
      {
        case ChartType.Regular:
        case ChartType.HeikinAshi:
          throw new Exception("This study can be applied only to one of the following Non-Standard Chart Types: Point & Figure, Kagi, Line Break, Renko.");
      }

      Color ArrowColor  = UpColor;
      if (Bars.UpTicks[0] < Bars.DownTicks[0] )
        ArrowColor = DnColor;

      plot1.Set(Bars.HighValue + UpMargin_pt * Bars.Info.MinMove / Bars.Info.PriceScale, ArrowColor);
      plot1.BGColor = ArrowColor;
   }
  }
}