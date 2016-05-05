Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Next_3rd_Friday
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.series = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, CDbl(MyBase.Bars.Next3rdFriday(Me.series, 0)))
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Next3F", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property series As Integer


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
