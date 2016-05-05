Imports PowerLanguage
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Outside_Bar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), MyBase.Bars.High.Item(1)) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), MyBase.Bars.Low.Item(1))) Then
                Me.Plot1.Set(0, MyBase.Bars.Close.Item(0))
                MyBase.Alerts.Alert
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Outside", EPlotShapes.Point, Color.Magenta, Color.Empty, 4, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
