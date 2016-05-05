Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Avg_Close_Less_Avg_Open
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleLess(Me.m_averagefc1.Item(0), Me.m_averagefc2.Item(0)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_averagefc2 = New [Function].AverageFC(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Red, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Red, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_averagefc1.price = MyBase.Bars.Close
            Me.m_averagefc1.length = Me.length
            Me.m_averagefc2.price = MyBase.Bars.Open
            Me.m_averagefc2.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        ' Fields
        Private m_averagefc1 As [Function].AverageFC
        Private m_averagefc2 As [Function].AverageFC
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
