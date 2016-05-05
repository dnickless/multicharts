Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Arms_Index_TRIN
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbought = 0.7
            Me.oversold = 1.25
            Me.smoothinglength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_trin.Value = ArmsIndex.Calc(Me.advissues, Me.advvol, Me.decissues, Me.decvol)
            Me.m_avgtrin.Value = PublicFunctions.Average(Me.m_trin, Me.smoothinglength)
            Me.Plot1.Set(0, Me.m_trin.Value)
            Me.Plot2.Set(0, Me.m_avgtrin.Value)
            Me.Plot3.Set(0, Me.oversold)
            Me.Plot4.Set(0, Me.overbought)
            If Me.CrossesUnder(Me.m_avgtrin, Me.oversold) Then
                MyBase.Alerts.Alert("Indicator exiting oversold zone", New Object(0  - 1) {})
            ElseIf Me.CrossesOver(Me.m_avgtrin, Me.overbought) Then
                MyBase.Alerts.Alert("Indicator exiting overbought zone", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_trin = New VariableSeries(Of Double)(Me)
            Me.m_avgtrin = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ArmsX", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ArmsXAvg", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.advissues = MyBase.Bars.Close
            Me.advvol = MyBase.BarsOfData(2).Close
            Me.decissues = MyBase.BarsOfData(3).Close
            Me.decvol = MyBase.BarsOfData(4).Close
        End Sub


        ' Properties
        Private Property advissues As ISeries(Of Double)

        Private Property advvol As ISeries(Of Double)

        Private Property decissues As ISeries(Of Double)

        Private Property decvol As ISeries(Of Double)

        <Input()> _
        Public Property overbought As Double

        <Input()> _
        Public Property oversold As Double

        <Input()> _
        Public Property smoothinglength As Integer


        ' Fields
        Private m_avgtrin As VariableSeries(Of Double)
        Private m_trin As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
