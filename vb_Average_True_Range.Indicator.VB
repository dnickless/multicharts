Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Average_True_Range
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.alertlength = 14
            Me.atrlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_atr.Value = Me.AverageTrueRange(Me.atrlength)
            Me.Plot1.Set(0, Me.m_atr.Value)
            If (Me.m_atr.HighestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at high", New Object(0  - 1) {})
            ElseIf (Me.m_atr.LowestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at low", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_atr = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ATR", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer

        <Input()> _
        Public Property atrlength As Integer


        ' Fields
        Private m_atr As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
