Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Cust_Gradient_Unbnd
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Cyan
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
            Me.Plot1.Set(0, MyBase.Bars.High.Item(0), m_colorlevel)
            Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), m_colorlevel)
            Me.Plot3.Set(0, MyBase.Bars.Open.Item(0), m_colorlevel)
            Me.Plot4.Set(0, MyBase.Bars.Close.Item(0), m_colorlevel)
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New [Function].NormGradientColor(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.White, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.White, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Plot3", EPlotShapes.LeftTick, Color.White, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("Plot4", EPlotShapes.RightTick, Color.White, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.formula = MyBase.Bars.Close
            Me.m_normgradientcolor1.dataseriesvalue = Me.formula
            Me.m_normgradientcolor1.crosseszero = Me.crosseszero
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
        End Sub


        ' Properties
        <Input> _
        Public Property colornormlength As Integer

        <Input()> _
        Public Property crosseszero As Boolean

        <Input()> _
        Public Property dncolor As Color

        Private Property formula As ISeries(Of Double)

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_normgradientcolor1 As [Function].NormGradientColor
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
