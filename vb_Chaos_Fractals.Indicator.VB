Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Chaos_Fractals
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.strength = 2
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_value1 As Double = Me.m_swinghigh1.Item(0)
            Dim m_value2 As Double = Me.m_swinglow1.Item(0)
            If PublicFunctions.DoubleGreater(m_value1, -1) Then
                Me.Plot1.Set(0, m_value1)
            End If
            If PublicFunctions.DoubleGreater(m_value2, -1) Then
                Me.Plot2.Set(0, m_value2)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_swinghigh1 = New SwingHigh(Me)
            Me.m_swinglow1 = New SwingLow(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BuyFractal", EPlotShapes.Point, Color.Lime, Color.Empty, 1, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("SellFractal", EPlotShapes.Point, ColorTranslator.FromWin32(&HE84FF0), Color.Empty, 1, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_swinghigh1.instance = 1
            Me.m_swinghigh1.price = MyBase.Bars.High
            Me.m_swinghigh1.strength = Me.strength
            Me.m_swinghigh1.length = 80
            Me.m_swinglow1.instance = 1
            Me.m_swinglow1.price = MyBase.Bars.Low
            Me.m_swinglow1.strength = Me.strength
            Me.m_swinglow1.length = 80
        End Sub


        ' Properties
        <Input> _
        Public Property strength As Integer


        ' Fields
        Private m_swinghigh1 As SwingHigh
        Private m_swinglow1 As SwingLow
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
