Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_PercentR
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbcolor = Color.Red
            Me.overscolor = Color.Cyan
            Me.overbought = 80
            Me.oversold = 20
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_pctr.Value = MyBase.Bars.PercentR(Me.length)
            Me.Plot1.Set(0, Me.m_pctr.Value)
            Me.Plot2.Set(0, Me.overbought)
            Me.Plot3.Set(0, Me.oversold)
            If PublicFunctions.DoubleGreater(Me.m_pctr.Value, Me.overbought) Then
                Me.Plot1.Colors.Item(0) = Me.overbcolor
            ElseIf PublicFunctions.DoubleLess(Me.m_pctr.Value, Me.oversold) Then
                Me.Plot1.Colors.Item(0) = Me.overscolor
            End If
            If Me.CrossesOver(Me.m_pctr, Me.oversold) Then
                MyBase.Alerts.Alert("Indicator exiting oversold zone", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_pctr, Me.overbought) Then
                MyBase.Alerts.Alert("Indicator exiting overbought zone", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_pctr = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("%R", EPlotShapes.Line, Color.Silver, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
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
        Private m_pctr As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
