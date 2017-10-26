Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Keltner_Channel
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.numatrs = 1.5
            Me.length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_avg As Double = Me.m_averagefc1.Item(0)
            Dim m_shift As Double = (Me.numatrs * Me.AverageTrueRange(Me.length))
            Me.m_upperband.Value = (m_avg + m_shift)
            Me.m_lowerband.Value = (m_avg - m_shift)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_upperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_lowerband.Value)
                Me.Plot3.Set(Me.displace, m_avg)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.price, Me.m_upperband) Then
                        MyBase.Alerts.Alert("Price crossing over upper band", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.price, Me.m_lowerband) Then
                        MyBase.Alerts.Alert("Price crossing under lower band", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("MidLine", EPlotShapes.Line, Color.Gray, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.price
            Me.m_averagefc1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property numatrs As Double

        Private ReadOnly Property price As ISeries(Of Double)
            Get
                Return Me.m_price
            End Get
        End Property


        ' Fields
        Private m_averagefc1 As [Function].AverageFC
        Private m_lowerband As VariableSeries(Of Double)
        Private m_price As ISeries(Of Double)
        Private m_upperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
