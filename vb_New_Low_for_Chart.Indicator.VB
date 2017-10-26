Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_Low_for_Chart
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_lowestlo) Then
                Me.Plot1.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
                Me.m_lowestlo = MyBase.Bars.Low.Item(0)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewLo-Ch", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_lowestlo = MyBase.Bars.Low.Item(0)
        End Sub


        ' Fields
        Private m_lowestlo As Double
        Private Plot1 As IPlotObject
    End Class
End Namespace
