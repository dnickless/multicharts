Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_On_Balance_Volume
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.alertlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_obvvalue.Value = Me.m_obv1.Item(0)
            Me.Plot1.Set(0, Me.m_obvvalue.Value)
            If ((MyBase.Bars.Close.LowestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_obvvalue.LowestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0  - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_obvvalue.HighestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_obv1 = New OBV(Me)
            Me.m_obvvalue = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("OBV", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer


        ' Fields
        Private m_obv1 As OBV
        Private m_obvvalue As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
