Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Stochastic_SlowK
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.overbcolor = Color.Blue
            Me.overscolor = Color.Cyan
            Me.overbought = 80
            Me.oversold = 20
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_myslowk As Double = Me.m_slowkcustom1.Item(0)
            If PublicFunctions.DoubleLess(m_myslowk, Me.oversold) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0), Me.overscolor)
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), Me.overscolor)
                MyBase.Alerts.Alert("SlowK in oversold zone", New Object(0  - 1) {})
            ElseIf PublicFunctions.DoubleGreater(m_myslowk, Me.overbought) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0), Me.overbcolor)
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), Me.overbcolor)
                MyBase.Alerts.Alert("SlowK in overbought zone", New Object(0  - 1) {})
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_slowkcustom1 = New SlowKCustom(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.White, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.White, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.priceh = MyBase.Bars.High
            Me.pricel = MyBase.Bars.Low
            Me.pricec = MyBase.Bars.Close
            Me.m_slowkcustom1.priceh = Me.priceh
            Me.m_slowkcustom1.pricel = Me.pricel
            Me.m_slowkcustom1.pricec = Me.pricec
            Me.m_slowkcustom1.stochlength = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property overbcolor As Color

        <Input()> _
        Public Property overbought As Double

        <Input()> _
        Public Property overscolor As Color

        <Input()> _
        Public Property oversold As Double

        Private Property pricec As ISeries(Of Double)

        Private Property priceh As ISeries(Of Double)

        Private Property pricel As ISeries(Of Double)

        ' Fields
        Private m_slowkcustom1 As SlowKCustom
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
