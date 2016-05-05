Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Brkout_of_X_Bar_Low
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            m_lowest.Value = Bars.Low.Lowest(Me.length)
            If PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), m_lowest.Item(1)) Then
                Me.Plot1.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert()
            End If
        End Sub

        Protected Overrides Sub Create()
            m_lowest = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BrkoutLo", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private Plot1 As IPlotObject
        Private m_lowest As VariableSeries(Of Double)
    End Class
End Namespace
