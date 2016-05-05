Imports PowerLanguage
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Custom_ShowMe
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (PublicFunctions.DoubleLess(MyBase.Bars.High.Item(0), MyBase.Bars.High.Item(1)) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), MyBase.Bars.Low.Item(1))) Then
                Me.Plot1.Set(0, MyBase.Bars.CloseValue)
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("CustomSM", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
