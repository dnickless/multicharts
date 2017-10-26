Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports PowerLanguage.Indicator
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Collections.Concurrent

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)>
    Public Class vb_Bollinger_Bands_Area
        Inherits IndicatorObject
        Implements IChartCustomDrawer
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.numdevsdn = -2
            Me.numdevsup = 2
            Me.length = 20
            fillcolor = Color.FromArgb(128, 128, 255)
            transparency = 85
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_avg As Double = Me.m_averagefc1.Item(0)
            Dim m_sdev As Double = Me.bollingerprice.StandardDeviationCustom(Me.length, 1)
            Me.m_upperband.Value = (m_avg + (Me.numdevsup * m_sdev))
            Me.m_lowerband.Value = (m_avg + (Me.numdevsdn * m_sdev))
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_upperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_lowerband.Value)
                Me.Plot3.Set(Me.displace, m_avg)

                m_IndicatorArea.SetValue("UpperBand", m_upperband.Value, Bars.TimeValue.ToBinary())
                m_IndicatorArea.SetValue("LowerBand", m_lowerband.Value, Bars.TimeValue.ToBinary())

                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.testpricelband, Me.m_lowerband) Then
                        MyBase.Alerts.Alert("Price crossing over lower price band", New Object(0 - 1) {})
                    ElseIf Me.CrossesUnder(Me.testpriceuband, Me.m_upperband) Then
                        MyBase.Alerts.Alert("Price crossing under upper price band", New Object(0 - 1) {})
                    End If
                End If
            End If
            ChartCustomDraw.ReDraw()
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New AverageFC(Me)
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Red, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("MidLine", EPlotShapes.Line, Color.Gray, Color.Empty, 0, 0, True))
            m_IndicatorArea = New vbAreaValue()
            isCSDRegistred = False
        End Sub

        Protected Overrides Sub StartCalc()
            If Not isCSDRegistred Then
                ChartCustomDraw.Register(Me)
                isCSDRegistred = True
            End If

            Me.bollingerprice = MyBase.Bars.Close
            Me.testpriceuband = MyBase.Bars.Close
            Me.testpricelband = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.bollingerprice
            Me.m_averagefc1.length = Me.length
        End Sub

        Protected Overrides Sub StopCalc()
            If isCSDRegistred Then
                ChartCustomDraw.Unregister(Me)
                isCSDRegistred = False
            End If
        End Sub

        Public Sub Draw(context As DrawContext, phase As EDrawPhases) Implements IChartCustomDrawer.Draw
            If phase <> EDrawPhases.Final Then
                Return
            End If

            Dim _dr As RectangleF = context.FullRect
            Dim DrPleft As ChartPoint = context.Environment.Point2ChartPoint(New PointF(_dr.X, _dr.Y))
            Dim DrPRight As ChartPoint = context.Environment.Point2ChartPoint(New PointF(_dr.Width, _dr.Height))

            If DrPleft.Time.Ticks > DrPRight.Time.Ticks Then Return

            Dim BarsToRedraw As Bar()
            context.Environment.Bars.Get(DrPleft.Time, DrPRight.Time, BarsToRedraw)

            If BarsToRedraw.Length > 0 Then
                Dim countBars As Integer = BarsToRedraw.Length
                Dim p As vbAreaPainter = New vbAreaPainter(fillcolor, 255 - m_intensity)
                Dim pf As PointF()
                ReDim pf(countBars * 2)
                Dim full As Integer = countBars * 2 - 1
                Dim idx As Integer = 0
                For i As Integer = 0 To countBars - 1
                    Dim high0 As Double = m_IndicatorArea.GetValue("UpperBand", BarsToRedraw(i).Time.ToBinary())
                    Dim low0 As Double = m_IndicatorArea.GetValue("LowerBand", BarsToRedraw(i).Time.ToBinary())

                    If high0 < 0 Or low0 < 0 Then
                        idx = 0
                        full -= 2
                        ReDim pf(full)
                        Continue For
                    End If

                    Dim b As Bar = BarsToRedraw(i)
                    pf(idx) = context.Environment.ChartPoint2Point(New ChartPoint(b.Time, high0))
                    pf(full - idx) = context.Environment.ChartPoint2Point(New ChartPoint(b.Time, low0))
                    idx += 1
                Next
                p.PaintArea(context.graphics, pf)
            End If
            context.DirtyRect = _dr
        End Sub


        ' Properties
        Private Property bollingerprice As ISeries(Of Double)

        <Input()>
        Public Property displace As Integer

        <Input()>
        Public Property length As Integer

        <Input()>
        Public Property numdevsdn As Double

        <Input()>
        Public Property numdevsup As Double

        <Input()>
        Public Property fillcolor As Color

        <Input()>
        Public Property transparency As Byte
            Get
                Return Convert.ToByte(m_intensity / 255.0 * 100.0)
            End Get
            Set(value As Byte)
                Try
                    m_intensity = Convert.ToByte(255.0 / 100.0 * value)
                Catch ex As Exception
                    transparency = 100
                End Try
            End Set
        End Property


        Private Property testpricelband As ISeries(Of Double)

        Private Property testpriceuband As ISeries(Of Double)


        ' Fields
        Private m_averagefc1 As AverageFC
        Private m_lowerband As VariableSeries(Of Double)
        Private m_upperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private m_IndicatorArea As vbAreaValue
        Private m_intensity As Byte
        Private isCSDRegistred As Boolean



        Class vbAreaValue
            Private m_Data As ConcurrentDictionary(Of String, ConcurrentDictionary(Of Long, Double))

            Public Sub New()
                m_Data = New ConcurrentDictionary(Of String, ConcurrentDictionary(Of Long, Double))
            End Sub

            Public Sub SetValue(Name As String, Value As Double, Time As Int64)
                If m_Data.ContainsKey(Name) Then
                    m_Data(Name)(Time) = Value
                Else
                    Dim data As ConcurrentDictionary(Of Long, Double) = New ConcurrentDictionary(Of Long, Double)
                    While (Not data.TryAdd(Time, Value))
                    End While

                    While (Not m_Data.TryAdd(Name, data))
                    End While
                End If
            End Sub

            Public Function GetValue(Name As String, Time As Int64) As Double
                Try
                    If m_Data(Name).ContainsKey(Time) Then
                        Return m_Data(Name)(Time)
                    End If
                Catch ex As Exception
                End Try

                Return -1
            End Function

            Public Sub Clear()
                m_Data.Clear()
            End Sub

        End Class

        Class vbAreaPainter
            Private m_Alpha As Byte
            Private m_FillColor As Color
            Private m_PainterPen As Pen
            Private m_SolidBrush As SolidBrush

            Public Sub New(color As Color, Alphavalue As Byte)
                m_FillColor = color
                m_Alpha = Alphavalue

                m_SolidBrush = New SolidBrush(Color.FromArgb(Alphavalue, m_FillColor))
                m_PainterPen = New Pen(m_SolidBrush)
            End Sub

            Public Sub PaintArea(g As Graphics, p As PointF())
                g.DrawPolygon(m_PainterPen, p)
                g.FillPolygon(m_SolidBrush, p)
            End Sub

        End Class

    End Class
End Namespace
