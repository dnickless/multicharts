Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Accum_Dist__PrVol
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.AlertLength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_accdstprvol.Value = Me.m_accumdist1.Item(0)
            Me.Plot1.Set(0, Me.m_accdstprvol.Value)
            If ((MyBase.Bars.Close.LowestBar(Me.AlertLength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_accdstprvol.LowestBar(Me.AlertLength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0  - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.AlertLength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_accdstprvol.HighestBar(Me.AlertLength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_accumdist1 = New AccumulationDistribution(Me)
            Me.m_accdstprvol = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("AccDst-PrVol", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_accumdist1.AnyVolume = MyBase.Bars.TrueVolume()
        End Sub


        ' Properties
        <Input> _
        Public Property AlertLength As Integer

        ' Fields
        Private m_accdstprvol As VariableSeries(Of Double)
        Private m_accumdist1 As AccumulationDistribution
        Private Plot1 As IPlotObject
    End Class
End Namespace
