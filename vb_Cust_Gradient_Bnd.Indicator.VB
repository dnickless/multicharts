Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Cust_Gradient_Bnd
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Cyan
            Me.upperbound = 100
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_formula As Double = Me.m_fastk1.Item(0)
            Dim m_colorlevel As Color = PublicFunctions.GradientColor(m_formula, Me.lowerbound, Me.upperbound, Me.dncolor, Me.upcolor)
            Me.Plot1.Set(0, MyBase.Bars.High.Item(0), m_colorlevel)
            Me.Plot2.Set(0, MyBase.Bars.Low.Item(0), m_colorlevel)
            Me.Plot3.Set(0, MyBase.Bars.Open.Item(0), m_colorlevel)
            Me.Plot4.Set(0, MyBase.Bars.Close.Item(0), m_colorlevel)
        End Sub

        Protected Overrides Sub Create()
            Me.m_fastk1 = New [Function].FastK(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.White, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.White, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Plot3", EPlotShapes.LeftTick, Color.White, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("Plot4", EPlotShapes.RightTick, Color.White, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_fastk1.stochlength = 14
        End Sub


        ' Properties
        <Input> _
        Public Property dncolor As Color

        <Input()> _
        Public Property lowerbound As Double

        <Input()> _
        Public Property upcolor As Color

        <Input()> _
        Public Property upperbound As Double


        ' Fields
        Private m_fastk1 As [Function].FastK
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
