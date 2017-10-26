using System;
using System.Drawing;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections;
using PowerLanguage.Function;
using System.Collections.Generic;

namespace PowerLanguage.Indicator
{
  [SameAsSymbol(true)]
  public class Bollinger_Bands_Area : IndicatorObject, IChartCustomDrawer
  {
    public Bollinger_Bands_Area(object _ctx)
      : base(_ctx)
    {
      numdevsdn = -2;
      numdevsup = 2;
      length = 20;

      fillcolor = Color.FromArgb(128, 128, 255);
      transparency = 85;
    }

    private AverageFC m_averagefc1;

    private VariableSeries<Double> m_lowerband;
    private VariableSeries<Double> m_upperband;

    private IPlotObject Plot1;
    private IPlotObject Plot2;
    private IPlotObject Plot3;

    private AreaValue m_IndicatorArea;

    private bool isCSDRegistred;
    private byte m_intensity; // 0-255

    private ISeries<double> bollingerprice { get; set; }

    private ISeries<double> testpriceuband { get; set; }

    private ISeries<double> testpricelband { get; set; }

    [Input]
    public int length { get; set; }

    [Input]
    public double numdevsup { get; set; }

    [Input]
    public double numdevsdn { get; set; }

    [Input]
    public int displace { get; set; }

    [Input]
    public Color fillcolor { get; set; }

    [Input]
    public byte transparency
    {
      get
      {
        byte res = Convert.ToByte((float)m_intensity / 255.0 * 100.0);
        return res;
      }
      set
      {
        try
        {
          m_intensity = Convert.ToByte(255.0 / 100.0 * (float)value);
        }
        catch (Exception)
        {
          transparency = 100;
        }
      }
    }

    protected override void Create()
    {
      m_averagefc1 = new AverageFC(this);
      m_lowerband = new VariableSeries<Double>(this);
      m_upperband = new VariableSeries<Double>(this);
      Plot1 =
          AddPlot(new PlotAttributes("UpperBand", 0, Color.Green, Color.Empty, 1, 0, true));
      Plot2 =
          AddPlot(new PlotAttributes("LowerBand", 0, Color.Red, Color.Empty, 1, 0, true));
      Plot3 =
          AddPlot(new PlotAttributes("MidLine", 0, Color.Gray, Color.Empty, 1, 0, true));

      m_IndicatorArea = new AreaValue();
      isCSDRegistred = false;
    }

    protected override void StartCalc()
    {
      if (!isCSDRegistred)
      {
        ChartCustomDraw.Register(this);
        isCSDRegistred = true;
      }

      bollingerprice = Bars.Close;
      testpriceuband = Bars.Close;
      testpricelband = Bars.Close;
      m_averagefc1.price = bollingerprice;
      m_averagefc1.length = length;
    }

    protected override void StopCalc()
    {
      if (isCSDRegistred)
      {
        ChartCustomDraw.Unregister(this);
        isCSDRegistred = false;
      }
    }

    protected override void CalcBar()
    {
      var m_avg = m_averagefc1[0];
      var m_sdev = bollingerprice.StandardDeviationCustom(length, 1);
      m_upperband.Value = (m_avg + (numdevsup * m_sdev));
      m_lowerband.Value = (m_avg + (numdevsdn * m_sdev));
      if (((displace >= 0) || Bars.CurrentBar > Math.Abs(displace)))
      {
        Plot1.Set(displace, m_upperband.Value);
        Plot2.Set(displace, m_lowerband.Value);
        Plot3.Set(displace, m_avg);

        m_IndicatorArea.SetValue("UpperBand", m_upperband.Value, Bars.TimeValue.ToBinary());
        m_IndicatorArea.SetValue("LowerBand", m_lowerband.Value, Bars.TimeValue.ToBinary());          

        if ((displace <= 0))
        {
          if (this.CrossesOver(testpricelband, m_lowerband))
          {
            Alerts.Alert("Price crossing over lower price band");
          }
          else
          {
            if (this.CrossesUnder(testpriceuband, m_upperband))
            {
              Alerts.Alert("Price crossing under upper price band");
            }
          }
        }
      }

      ChartCustomDraw.ReDraw();
    }


    #region IChartCustomDrawer
    void IChartCustomDrawer.Draw(DrawContext context, EDrawPhases phase)
    {
      if (phase != EDrawPhases.Final) return;

      RectangleF _dr = context.FullRect;
      ChartPoint DrPleft = context.Environment.Point2ChartPoint(new PointF { X = _dr.X, Y = _dr.Y });
      ChartPoint DrPRight = context.Environment.Point2ChartPoint(new PointF { X = _dr.Width, Y = _dr.Height });

      if (DrPleft.Time.Ticks > DrPRight.Time.Ticks)
      {
        return;
      }

      Bar[] BarsToRedraw = null;

      context.Environment.Bars.Get(DrPleft.Time, DrPRight.Time, out BarsToRedraw);     
      if (!object.Equals(BarsToRedraw, null))
      {
        int countBars = BarsToRedraw.Length;
        if (countBars > 0)
        {
          AreaPainter p = new AreaPainter(fillcolor, (byte)(255 - m_intensity));
          PointF[] pf = new PointF[countBars * 2];
          int full = countBars * 2 - 1;
          for (int i = 0, idx = 0; i < countBars; i++)
          {
            double high0 = m_IndicatorArea.GetValue("UpperBand", BarsToRedraw[i].Time.ToBinary());
            double low0 = m_IndicatorArea.GetValue("LowerBand", BarsToRedraw[i].Time.ToBinary());

            if (high0 < 0|| low0 < 0)
            {
              idx = 0;
              full-=2;
              pf = new PointF[full + 1];
              continue;
            }

            Bar b = BarsToRedraw[i];
            pf[idx] = context.Environment.ChartPoint2Point(new ChartPoint
            {
              Price = high0,
              Time = b.Time
            });

            pf[full - idx] = context.Environment.ChartPoint2Point(new ChartPoint
            {
              Price = low0,
              Time = b.Time
            });
            idx++;
          }
          p.PaintArea(context.graphics, pf);
        }
      }
      context.DirtyRect = _dr;
    }
    #endregion

  }

  public class AreaValue
  {
    
   ConcurrentDictionary<String, ConcurrentDictionary<long /*Datetime*/, double>> m_Data;

    public AreaValue()
    {
      m_Data = new ConcurrentDictionary<string,ConcurrentDictionary<long, double>>();
    }

    public void SetValue(String Name, double Value, long Time)
    {
      if(m_Data.ContainsKey(Name))
      {
        m_Data[Name][Time] = Value;
      }
      else
      {
        ConcurrentDictionary<long, double> data = new ConcurrentDictionary<long, double>();
        while(!data.TryAdd(Time, Value))
        { }

        while(!m_Data.TryAdd(Name, data))
        { }
      }
    }

    public double GetValue(String Name, long Time)
    {
      try
      {
        if(m_Data[Name].ContainsKey(Time))
          return m_Data[Name][Time];

        return -1;
      }
      catch(System.Exception)
      {
        return -1;
      }
    }

    public void Clear()
    {
      m_Data.Clear();
    }
  }

  public class AreaPainter
  {
    byte m_Alpha;  //0->255
    Color m_FillColor;

    Pen m_PainterPen;
    SolidBrush m_SolidBrush;
    public AreaPainter(Color color, byte Alphavalue)
    {
      m_FillColor = color;
      m_Alpha = Alphavalue;

      m_SolidBrush = new SolidBrush(Color.FromArgb(m_Alpha, m_FillColor));
      m_PainterPen = new Pen(m_SolidBrush);
    }

    public void PaintArea(Graphics g, PointF[] p)
    {
      g.DrawPolygon(m_PainterPen, p);
      g.FillPolygon(m_SolidBrush, p);
    }

  }
}
