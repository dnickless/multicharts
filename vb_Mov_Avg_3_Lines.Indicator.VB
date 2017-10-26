Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_3_Lines
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.slowlength = &H12
            Me.medlength = 9
            Me.fastlength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_fastavg As Double = Me.m_averagefc1.Item(0)
            Dim m_medavg As Double = Me.m_averagefc2.Item(0)
            Dim m_slowavg As Double = Me.m_averagefc3.Item(0)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, m_fastavg)
                Me.Plot2.Set(Me.displace, m_medavg)
                Me.Plot3.Set(Me.displace, m_slowavg)
                If (Me.displace <= 0) Then
                    Me.m_condition1.Value = ((PublicFunctions.DoubleGreater(Me.price.Item(0), m_fastavg) AndAlso PublicFunctions.DoubleGreater(m_fastavg, m_medavg)) AndAlso PublicFunctions.DoubleGreater(m_medavg, m_slowavg))
                    If Not If(Not Me.m_condition1.Value, True, Me.m_condition1.Item(1)) Then
                        MyBase.Alerts.Alert("Bullish alert", New Object(0  - 1) {})
                    Else
                        Me.m_condition2.Value = ((PublicFunctions.DoubleLess(Me.price.Item(0), m_fastavg) AndAlso PublicFunctions.DoubleLess(m_fastavg, m_medavg)) AndAlso PublicFunctions.DoubleLess(m_medavg, m_slowavg))
                        If Not If(Not Me.m_condition2.Value, True, Me.m_condition2.Item(1)) Then
                            MyBase.Alerts.Alert("Bearish alert", New Object(0  - 1) {})
                        End If
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_averagefc2 = New [Function].AverageFC(Me)
            Me.m_averagefc3 = New [Function].AverageFC(Me)
            Me.m_condition1 = New VariableSeries(Of Boolean)(Me)
            Me.m_condition2 = New VariableSeries(Of Boolean)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("FastAvg", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("MedAvg", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("SlowAvg", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.price
            Me.m_averagefc1.length = Me.fastlength
            Me.m_averagefc2.price = Me.price
            Me.m_averagefc2.length = Me.medlength
            Me.m_averagefc3.price = Me.price
            Me.m_averagefc3.length = Me.slowlength
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property fastlength As Integer

        <Input()> _
        Public Property medlength As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property slowlength As Integer


        ' Fields
        Private m_averagefc1 As [Function].AverageFC
        Private m_averagefc2 As [Function].AverageFC
        Private m_averagefc3 As [Function].AverageFC
        Private m_condition1 As VariableSeries(Of Boolean)
        Private m_condition2 As VariableSeries(Of Boolean)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
