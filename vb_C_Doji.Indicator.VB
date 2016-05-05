Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_Doji
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.percent = 5
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Bars.Doji(Me.percent, 0) Then
                Me.Plot1.Set(0, MyBase.Bars.Close.Item(0))
                MyBase.Alerts.Alert("Doji", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Doji", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property percent As Double


        ' Fields
        Private Plot1 As IPlotObject
    End Class
End Namespace
