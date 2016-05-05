Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Stochastic_Slow
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbought = 80
            Me.oversold = 20
            Me.smoothingtype = 1
            Me.smoothinglength2 = 3
            Me.smoothinglength1 = 3
            Me.stochlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_stochastic1.Call
            Me.Plot1.Set(0, Me.m_oslowk.Value)
            Me.Plot2.Set(0, Me.m_oslowd.Value)
            Me.Plot3.Set(0, Me.overbought)
            Me.Plot4.Set(0, Me.oversold)
            If (MyBase.Bars.CurrentBar > 2) Then
                If (Me.CrossesOver(Me.m_oslowk, Me.m_oslowd) AndAlso PublicFunctions.DoubleLess(Me.m_oslowk.Value, Me.oversold)) Then
                    MyBase.Alerts.Alert("SlowK crossing over SlowD", New Object(0  - 1) {})
                ElseIf (Me.CrossesUnder(Me.m_oslowk, Me.m_oslowd) AndAlso PublicFunctions.DoubleGreater(Me.m_oslowk.Value, Me.overbought)) Then
                    MyBase.Alerts.Alert("SlowK crossing under SlowD", New Object(0  - 1) {})
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_stochastic1 = New [Function].Stochastic(Me)
            Me.m_ofastk = New VariableSeries(Of Double)(Me)
            Me.m_ofastd = New VariableSeries(Of Double)(Me)
            Me.m_oslowk = New VariableSeries(Of Double)(Me)
            Me.m_oslowd = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("SlowK", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("SlowD", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("OverBot", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("OverSld", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.priceh = MyBase.Bars.High
            Me.pricel = MyBase.Bars.Low
            Me.pricec = MyBase.Bars.Close
            Me.m_stochastic1.priceh = Me.priceh
            Me.m_stochastic1.pricel = Me.pricel
            Me.m_stochastic1.pricec = Me.pricec
            Me.m_stochastic1.stochlength = Me.stochlength
            Me.m_stochastic1.length1 = Me.smoothinglength1
            Me.m_stochastic1.length2 = Me.smoothinglength2
            Me.m_stochastic1.smoothingtype = Me.smoothingtype
            Me.m_stochastic1.ofastk = Me.m_ofastk
            Me.m_stochastic1.ofastd = Me.m_ofastd
            Me.m_stochastic1.oslowk = Me.m_oslowk
            Me.m_stochastic1.oslowd = Me.m_oslowd
        End Sub


        ' Properties
        <Input> _
        Public Property overbought As Double

        <Input()> _
        Public Property oversold As Double

        Private Property pricec As ISeries(Of Double)

        Private Property priceh As ISeries(Of Double)

        Private Property pricel As ISeries(Of Double)

        <Input()> _
        Public Property smoothinglength1 As Integer

        <Input()> _
        Public Property smoothinglength2 As Integer

        <Input()> _
        Public Property smoothingtype As Integer

        <Input()> _
        Public Property stochlength As Integer


        ' Fields
        Private m_ofastd As VariableSeries(Of Double)
        Private m_ofastk As VariableSeries(Of Double)
        Private m_oslowd As VariableSeries(Of Double)
        Private m_oslowk As VariableSeries(Of Double)
        Private m_stochastic1 As [Function].Stochastic
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
