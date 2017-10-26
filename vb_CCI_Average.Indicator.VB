Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_CCI_Average
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbought = 100
            Me.oversold = -100
            Me.cciavglength = 9
            Me.ccilength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ccivalue.Value = MyBase.Bars.CCI(Me.ccilength)
            Me.m_cciavg.Value = PublicFunctions.Average(Me.m_ccivalue, Me.cciavglength)
            Me.Plot1.Set(0, Me.m_ccivalue.Value)
            Me.Plot2.Set(0, Me.m_cciavg.Value)
            Me.Plot3.Set(0, Me.overbought)
            Me.Plot4.Set(0, Me.oversold)
            If Me.CrossesOver(Me.m_cciavg, Me.oversold) Then
                MyBase.Alerts.Alert("Indicator exiting oversold zone", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_cciavg, Me.overbought) Then
                MyBase.Alerts.Alert("Indicator exiting overbought zone", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ccivalue = New VariableSeries(Of Double)(Me)
            Me.m_cciavg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("CCI", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("CCIAvg", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property cciavglength As Integer

        <Input()> _
        Public Property ccilength As Integer

        <Input()> _
        Public Property overbought As Double

        <Input()> _
        Public Property oversold As Double


        ' Fields
        Private m_cciavg As VariableSeries(Of Double)
        Private m_ccivalue As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
