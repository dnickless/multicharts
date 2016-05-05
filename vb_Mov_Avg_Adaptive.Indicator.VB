Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Adaptive
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.slowavglength = 30
            Me.fastavglength = 2
            Me.effratiolength = 10
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_maa.Value = Me.m_adaptivemovavg1.Item(0)
            Me.Plot1.Set(0, Me.m_maa.Value)
            If Me.CrossesOver(Me.price, Me.m_maa) Then
                MyBase.Alerts.Alert("Bullish alert", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.price, Me.m_maa) Then
                MyBase.Alerts.Alert("Bearish alert", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_adaptivemovavg1 = New AdaptiveMovAvg(Me)
            Me.m_maa = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MAA", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_adaptivemovavg1.price = Me.price
            Me.m_adaptivemovavg1.effratiolength = Me.effratiolength
            Me.m_adaptivemovavg1.fastavglength = Me.fastavglength
            Me.m_adaptivemovavg1.slowavglength = Me.slowavglength
        End Sub


        ' Properties
        <Input> _
        Public Property effratiolength As Integer

        <Input()> _
        Public Property fastavglength As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property slowavglength As Integer


        ' Fields
        Private m_adaptivemovavg1 As AdaptiveMovAvg
        Private m_maa As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
