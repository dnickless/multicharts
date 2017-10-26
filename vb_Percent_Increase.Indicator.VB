Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Percent_Increase
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.pctup = 2
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreaterEquals(MyBase.Bars.Close.RateOfChange(Me.length), Me.pctup) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property pctup As Double


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
