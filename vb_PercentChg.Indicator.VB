Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_PercentChg
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Red
            Me.upcolor = Color.Cyan
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_pctchg.Value = (100 * MyBase.Bars.Close.PercentChange(Me.length, 0))
            Me.Plot1.Set(0, Me.m_pctchg.Value)
            Me.Plot2.Set(0, Me.hialert)
            Me.Plot3.Set(0, Me.loalert)
            If PublicFunctions.DoubleGreater(Me.m_pctchg.Value, Me.hialert) Then
                Me.Plot1.Colors.Item(0) = Me.upcolor
            ElseIf PublicFunctions.DoubleLess(Me.m_pctchg.Value, Me.loalert) Then
                Me.Plot1.Colors.Item(0) = Me.dncolor
            End If
            If Me.CrossesOver(Me.m_pctchg, Me.hialert) Then
                MyBase.Alerts.Alert("Bullish alert", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_pctchg, Me.loalert) Then
                MyBase.Alerts.Alert("Bearish alert", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_pctchg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("%Chg", EPlotShapes.Line, Color.Silver, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("HiAlert", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("LoAlert", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property dncolor As Color

        <Input()> _
        Public Property hialert As Double

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property loalert As Double

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_pctchg As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
