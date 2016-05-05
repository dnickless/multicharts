Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Envelopes
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.pctbelow = 1
            Me.pctabove = 1
            Me.length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_lowerband.Value = (Me.m_averagefc1.Item(0) * Me.m_pctbelowfactor)
            Me.m_upperband.Value = (Me.m_averagefc2.Item(0) * Me.m_pctabovefactor)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_upperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_lowerband.Value)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.pricel, Me.m_lowerband) Then
                        MyBase.Alerts.Alert("Price crossing over lower price band", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.priceh, Me.m_upperband) Then
                        MyBase.Alerts.Alert("Price crossing under upper price band", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_averagefc2 = New [Function].AverageFC(Me)
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.priceh = MyBase.Bars.Close
            Me.pricel = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.pricel
            Me.m_averagefc1.length = Me.length
            Me.m_averagefc2.price = Me.priceh
            Me.m_averagefc2.length = Me.length
            Me.m_lowerband.DefaultValue = 0
            Me.m_upperband.DefaultValue = 0
            Me.m_pctbelowfactor = (1 - (Me.pctbelow * 0.01))
            Me.m_pctabovefactor = (1 + (Me.pctabove * 0.01))
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property pctabove As Double

        <Input()> _
        Public Property pctbelow As Double

        Private Property priceh As ISeries(Of Double)

        Private Property pricel As ISeries(Of Double)


        ' Fields
        Private m_averagefc1 As [Function].AverageFC
        Private m_averagefc2 As [Function].AverageFC
        Private m_lowerband As VariableSeries(Of Double)
        Private m_pctabovefactor As Double
        Private m_pctbelowfactor As Double
        Private m_upperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
