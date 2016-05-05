Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_AdvanceDecl_Ratio
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, MyBase.Bars.Close.AdvanceDeclineRatio(MyBase.BarsOfData(2).Close))
            Me.Plot2.Set(0, 1)
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("A/DRatio", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Baseline", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
