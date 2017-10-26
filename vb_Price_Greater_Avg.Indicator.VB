Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Price_Greater_Avg
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(Me.price.Item(0), Me.m_averagefc1.Item(0)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New AverageFC(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.price
            Me.m_averagefc1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_averagefc1 As AverageFC
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
