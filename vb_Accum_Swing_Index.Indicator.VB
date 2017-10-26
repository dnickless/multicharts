Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Accum_Swing_Index
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_accumswingindex1.Item(0))
        End Sub

        Protected Overrides Sub Create()
            Me.m_accumswingindex1 = New AccumSwingIndex(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ASX", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub


        ' Fields
        Private m_accumswingindex1 As AccumSwingIndex
        Private Plot1 As IPlotObject
    End Class
End Namespace
