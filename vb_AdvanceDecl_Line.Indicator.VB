Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_AdvanceDecl_Line
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_advancedeclinediff1.Item(0))
        End Sub

        Protected Overrides Sub Create()
            Me.m_advancedeclinediff1 = New AdvanceDeclineDiff(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("A/DLine", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_advancedeclinediff1.advissues = MyBase.Bars.Close
            Me.m_advancedeclinediff1.decissues = MyBase.BarsOfData(2).Close
        End Sub


        ' Fields
        Private m_advancedeclinediff1 As AdvanceDeclineDiff
        Private Plot1 As IPlotObject
    End Class
End Namespace
