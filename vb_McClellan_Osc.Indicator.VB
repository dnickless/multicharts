Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_McClellan_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbought = 70
            Me.oversold = -70
            Me.slowlength = &H27
            Me.fastlength = &H13
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mcclosc.Value = Me.m_mcclellanosc1.Item(0)
            Me.Plot1.Set(0, Me.m_mcclosc.Value)
            Me.Plot2.Set(0, Me.overbought)
            Me.Plot3.Set(0, Me.oversold)
            If Me.CrossesOver(Me.m_mcclosc, Me.oversold) Then
                MyBase.Alerts.Alert("Indicator exiting oversold zone", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_mcclosc, Me.overbought) Then
                MyBase.Alerts.Alert("Indicator exiting overbought zone", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mcclellanosc1 = New [Function].McClellanOsc(Me)
            Me.m_mcclosc = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("McClOsc", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.advissues = MyBase.Bars.Close
            Me.decissues = MyBase.BarsOfData(2).Close
            Me.m_mcclellanosc1.advissues = Me.advissues
            Me.m_mcclellanosc1.decissues = Me.decissues
            Me.m_mcclellanosc1.fastlength = Me.fastlength
            Me.m_mcclellanosc1.slowlength = Me.slowlength
        End Sub


        ' Properties
        Private Property advissues As ISeries(Of Double)

        Private Property decissues As ISeries(Of Double)

        <Input()> _
        Public Property fastlength As Integer

        <Input()> _
        Public Property overbought As Double

        <Input()> _
        Public Property oversold As Double

        <Input()> _
        Public Property slowlength As Integer


        ' Fields
        Private m_mcclellanosc1 As [Function].McClellanOsc
        Private m_mcclosc As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
