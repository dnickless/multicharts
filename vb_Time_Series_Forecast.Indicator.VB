Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Time_Series_Forecast
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.barsplus = 7
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_tsf.Value = Me.m_timeseriesforecast1.Item(0)
            Me.Plot1.Set(0, Me.m_tsf.Value)
            If ((PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(0), Me.m_tsf.Value) AndAlso PublicFunctions.DoubleGreater(Me.m_tsf.Value, Me.m_tsf.Item(1))) AndAlso PublicFunctions.DoubleLessEquals(Me.m_tsf.Item(1), Me.m_tsf.Item(2))) Then
                MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
            ElseIf ((PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), Me.m_tsf.Value) AndAlso PublicFunctions.DoubleLess(Me.m_tsf.Value, Me.m_tsf.Item(1))) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_tsf.Item(1), Me.m_tsf.Item(2))) Then
                MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_timeseriesforecast1 = New [Function].TimeSeriesForecast(Me)
            Me.m_tsf = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("TSF", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_timeseriesforecast1.length = Me.length
            Me.m_timeseriesforecast1.tgtbar = Me.barsplus
        End Sub


        ' Properties
        <Input> _
        Public Property barsplus As Integer
        <Input()> _
        Public Property length As Integer

        ' Fields
        Private m_timeseriesforecast1 As [Function].TimeSeriesForecast
        Private m_tsf As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
