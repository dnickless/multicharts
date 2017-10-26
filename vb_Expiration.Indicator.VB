Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Expiration
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Bars.Next3rdFriday(1, 1) = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Magenta, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
