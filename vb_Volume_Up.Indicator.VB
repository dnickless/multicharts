Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Volume_Up
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((EResolution.Quarter <> resolution) AndAlso ((EResolution.Day > resolution) OrElse (resolution > EResolution.Year))) Then
                Me.Plot1.Set(0, MyBase.Bars.UpTicks.Item(0))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VolUp", EPlotShapes.Histogram, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
