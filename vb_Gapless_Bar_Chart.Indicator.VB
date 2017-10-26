Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Gapless_Bar_Chart
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, MyBase.Bars.TrueHigh(0))
            Me.Plot2.Set(0, MyBase.Bars.TrueLow(0))
            Me.Plot3.Set(0, MyBase.Bars.Open.Item(0))
            Me.Plot4.Set(0, MyBase.Bars.Close.Item(0))
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("TrueHigh", EPlotShapes.BarHigh, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("TrueLow", EPlotShapes.BarLow, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Open", EPlotShapes.LeftTick, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("Close", EPlotShapes.RightTick, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
