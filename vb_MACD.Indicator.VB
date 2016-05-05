Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_MACD
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.macdlength = 9
            Me.slowlength = &H1A
            Me.fastlength = 12
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mymacd.Value = Me.m_macd1.Item(0)
            Dim m_macdavg As Double = Me.m_xaverage1.Item(0)
            Me.m_macddiff.Value = (Me.m_mymacd.Value - m_macdavg)
            Me.Plot1.Set(0, Me.m_mymacd.Value)
            Me.Plot2.Set(0, m_macdavg)
            Me.Plot3.Set(0, Me.m_macddiff.Value)
            Me.Plot4.Set(0, 0)
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_macddiff, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Bullish alert", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_macddiff, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Bearish alert", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_macd1 = New [Function].MACD(Me)
            Me.m_xaverage1 = New XAverage(Me)
            Me.m_mymacd = New VariableSeries(Of Double)(Me)
            Me.m_macddiff = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vb_MACD", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("vb_MACDAvg", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("vb_MACDDiff", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_macd1.Price = MyBase.Bars.Close
            Me.m_macd1.FastLength = Me.fastlength
            Me.m_macd1.SlowLength = Me.slowlength
            Me.m_xaverage1.Price = Me.m_mymacd
            Me.m_xaverage1.Length = Me.macdlength
            Me.m_mymacd.DefaultValue = 0
            Me.m_macddiff.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property fastlength As Integer

        <Input()> _
        Public Property macdlength As Integer

        <Input()> _
        Public Property slowlength As Integer


        ' Fields
        Private m_macd1 As [Function].MACD
        Private m_macddiff As VariableSeries(Of Double)
        Private m_mymacd As VariableSeries(Of Double)
        Private m_xaverage1 As XAverage
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
