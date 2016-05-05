Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_DMI
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.adxtrend = 25
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_dirmovement1.Call
            Me.Plot1.Set(0, Me.m_dirmovement1.DMIPlus.Value)
            Me.Plot2.Set(0, Me.m_dirmovement1.DMIMinus.Value)
            Me.Plot3.Set(0, Me.m_dirmovement1.ADX.Value)
            If PublicFunctions.DoubleGreater(Me.m_dirmovement1.ADX.Value, Me.adxtrend) Then
                If Me.CrossesOver(Me.m_dirmovement1.DMIPlus, Me.m_dirmovement1.DMIMinus) Then
                    MyBase.Alerts.Alert("Bullish alert", New Object(0  - 1) {})
                ElseIf Me.CrossesUnder(Me.m_dirmovement1.DMIPlus, Me.m_dirmovement1.DMIMinus) Then
                    MyBase.Alerts.Alert("Bearish alert", New Object(0  - 1) {})
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_dirmovement1 = New DirMovement(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vb_DMI+", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("vb_DMI-", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("ADX", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_dirmovement1.PriceH = MyBase.Bars.High
            Me.m_dirmovement1.PriceL = MyBase.Bars.Low
            Me.m_dirmovement1.PriceC = MyBase.Bars.Close
            Me.m_dirmovement1.Length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property adxtrend As Double

        <Input()> _
        Public Property length As Integer


        ' Fields
        Private m_dirmovement1 As DirMovement
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
