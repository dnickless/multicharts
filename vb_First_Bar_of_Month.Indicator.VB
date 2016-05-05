Imports PowerLanguage
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_First_Bar_of_Month
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If (((resolution <> EResolution.Year) AndAlso (resolution <> EResolution.Quarter)) AndAlso (resolution <> EResolution.Month)) Then
                If (Bars.Time.Item(0).Month <> Bars.Time.Item(1).Month) Then
                    Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                    Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                    MyBase.Alerts.Alert()
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Magenta, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
