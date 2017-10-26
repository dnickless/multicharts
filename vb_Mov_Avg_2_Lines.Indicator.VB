Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_2_Lines
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.slowlength = &H12
            Me.fastlength = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_fastavg.Value = Me.m_averagefc1.Item(0)
            Me.m_slowavg.Value = Me.m_averagefc2.Item(0)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_fastavg.Value)
                Me.Plot2.Set(Me.displace, Me.m_slowavg.Value)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.m_fastavg, Me.m_slowavg) Then
                        MyBase.Alerts.Alert("Bullish alert", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.m_fastavg, Me.m_slowavg) Then
                        MyBase.Alerts.Alert("Bearish alert", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New AverageFC(Me)
            Me.m_averagefc2 = New AverageFC(Me)
            Me.m_fastavg = New VariableSeries(Of Double)(Me)
            Me.m_slowavg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("FastAvg", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("SlowAvg", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.price
            Me.m_averagefc1.length = Me.fastlength
            Me.m_averagefc2.price = Me.price
            Me.m_averagefc2.length = Me.slowlength
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property fastlength As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property slowlength As Integer


        ' Fields
        Private m_averagefc1 As AverageFC
        Private m_averagefc2 As AverageFC
        Private m_fastavg As VariableSeries(Of Double)
        Private m_slowavg As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
