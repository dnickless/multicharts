Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Accum_Dist__BuyPr
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.alertlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_accdstbuypr.Value = Me.m_lwaccdis1.Item(0)
            Me.Plot1.Set(0, Me.m_accdstbuypr.Value)
            If ((MyBase.Bars.Close.LowestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_accdstbuypr.LowestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0 - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_accdstbuypr.HighestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_lwaccdis1 = New LWAccDis(Me)
            Me.m_accdstbuypr = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("AccDst-BuyPr", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer

        ' Fields
        Private m_accdstbuypr As VariableSeries(Of Double)
        Private m_lwaccdis1 As LWAccDis
        Private Plot1 As IPlotObject
    End Class
End Namespace
