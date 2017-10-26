Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Momentum_Increasing
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.momlength = 14
            Me.momavglength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_momavg.Value = PublicFunctions.Average(Me.price.MomentumSeries(Me.momlength), Me.momavglength)
            If PublicFunctions.DoubleGreater(Me.m_momavg.Value, Me.m_momavg.Item(1)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_momavg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property momavglength As Integer

        <Input()> _
        Public Property momlength As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_momavg As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
