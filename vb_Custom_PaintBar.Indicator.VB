Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Custom_PaintBar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.criteria.Item(0) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Magenta, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Magenta, Color.Empty, 1, 1, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.criteria = New Lambda(Of Boolean)(Function(_bb) (PublicFunctions.DoubleLess(MyBase.Bars.High.Item(_bb), MyBase.Bars.High.Item((1 + _bb))) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(_bb), MyBase.Bars.Low.Item((1 + _bb)))))
        End Sub


        ' Properties
        Private Property criteria As ISeries(Of Boolean)


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
