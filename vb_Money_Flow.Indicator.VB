Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Money_Flow
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbcolor = Color.Red
            Me.overscolor = Color.Cyan
            Me.overbought = 80
            Me.oversold = 20
            Me.alertlength = 14
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_moneyflowval.Value = MyBase.Bars.MoneyFlow(Me.length)
            Me.Plot1.Set(0, Me.m_moneyflowval.Value)
            Me.Plot2.Set(0, Me.overbought)
            Me.Plot3.Set(0, Me.oversold)
            If PublicFunctions.DoubleGreater(Me.m_moneyflowval.Value, Me.overbought) Then
                Me.Plot1.Colors.Item(0) = Me.overbcolor
            ElseIf PublicFunctions.DoubleLess(Me.m_moneyflowval.Value, Me.oversold) Then
                Me.Plot1.Colors.Item(0) = Me.overscolor
            End If
            If ((MyBase.Bars.Close.LowestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_moneyflowval.LowestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0  - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_moneyflowval.HighestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_moneyflowval = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MoneyFlow", EPlotShapes.Line, ColorTranslator.FromWin32(&HA4A0A0), Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property overbcolor As Color

        <Input()> _
        Public Property overbought As Double

        <Input()> _
        Public Property overscolor As Color

        <Input()> _
        Public Property oversold As Double


        ' Fields
        Private m_moneyflowval As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
