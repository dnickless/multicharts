Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Volume
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_volume.Value)
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vb_Volume", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_volume = MyBase.Bars.TrueVolume()
        End Sub


        ' Fields
        Private m_volume As ISeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
