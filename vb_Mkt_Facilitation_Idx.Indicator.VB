Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Mkt_Facilitation_Idx
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.alertlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mktfacilx.Value = MyBase.Bars.MFI()
            Me.Plot1.Set(0, Me.m_mktfacilx.Value)
            If (Me.m_mktfacilx.HighestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at high", New Object(0  - 1) {})
            ElseIf (Me.m_mktfacilx.LowestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at low", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mktfacilx = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MktFacilX", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer


        ' Fields
        Private m_mktfacilx As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
