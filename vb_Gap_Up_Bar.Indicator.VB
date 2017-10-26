Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Gap_Up_Bar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), MyBase.Bars.High.Item(1)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("GapUp", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
