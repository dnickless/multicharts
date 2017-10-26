Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Open_Interest
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (Bars.OpenInt.Item(0) <> 0) Then
                Me.Plot1.Set(0, MyBase.Bars.OpenInt.Item(0))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("OpenInt", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
