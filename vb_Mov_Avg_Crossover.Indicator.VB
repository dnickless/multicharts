Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Crossover
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Blue
            Me.upcolor = Color.Cyan
            Me.slowlength = 14
            Me.fastlength = 7
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_fastavg As Double = PublicFunctions.Average(Me.price, Me.fastlength)
            Dim m_slowavg As Double = PublicFunctions.Average(Me.price, Me.slowlength)
            If PublicFunctions.DoubleGreater(m_fastavg, m_slowavg) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0), Me.upcolor)
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), Me.upcolor)
                MyBase.Alerts.Alert("FastAvg above SlowAvg", New Object(0  - 1) {})
            ElseIf PublicFunctions.DoubleLess(m_fastavg, m_slowavg) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0), Me.dncolor)
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), Me.dncolor)
                MyBase.Alerts.Alert("FastAvg below SlowAvg", New Object(0  - 1) {})
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.White, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.White, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property dncolor As Color

        <Input()> _
        Public Property fastlength As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property slowlength As Integer

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
