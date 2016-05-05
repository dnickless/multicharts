Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Bollinger_Bands
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.numdevsdn = -2
            Me.numdevsup = 2
            Me.length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_avg As Double = Me.m_averagefc1.Item(0)
            Dim m_sdev As Double = Me.bollingerprice.StandardDeviationCustom(Me.length, 1)
            Me.m_upperband.Value = (m_avg + (Me.numdevsup * m_sdev))
            Me.m_lowerband.Value = (m_avg + (Me.numdevsdn * m_sdev))
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_upperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_lowerband.Value)
                Me.Plot3.Set(Me.displace, m_avg)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.testpricelband, Me.m_lowerband) Then
                        MyBase.Alerts.Alert("Price crossing over lower price band", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.testpriceuband, Me.m_upperband) Then
                        MyBase.Alerts.Alert("Price crossing under upper price band", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New AverageFC(Me)
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("MidLine", EPlotShapes.Line, Color.Gray, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.bollingerprice = MyBase.Bars.Close
            Me.testpriceuband = MyBase.Bars.Close
            Me.testpricelband = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.bollingerprice
            Me.m_averagefc1.length = Me.length
        End Sub


        ' Properties
        Private Property bollingerprice As ISeries(Of Double)

        <Input()> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property numdevsdn As Double

        <Input()> _
        Public Property numdevsup As Double

        Private Property testpricelband As ISeries(Of Double)

        Private Property testpriceuband As ISeries(Of Double)


        ' Fields
        Private m_averagefc1 As AverageFC
        Private m_lowerband As VariableSeries(Of Double)
        Private m_upperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
