Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Detrended_Price_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_dpo.Value = MyBase.Bars.Close.Detrend(Me.length)
            Me.Plot1.Set(0, Me.m_dpo.Value)
            Me.Plot2.Set(0, 0)
            Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
            If (Me.m_applicationtype = 1) Then
                Me.Plot1.Colors.Item(0) = m_colorlevel
            ElseIf (Me.m_applicationtype > 1) Then
                Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                Me.Plot1.BGColor = m_colorlevel
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_dpo = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("DPO", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_dpo
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
            Me.m_applicationtype = CInt(MyBase.Environment.ApplicationCode)
        End Sub


        ' Properties
        <Input> _
        Public Property colornormlength As Integer

        <Input()> _
        Public Property dncolor As Color

        <Input()> _
        Public Property gridforegroundcolor As Color

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_applicationtype As Integer
        Private m_dpo As VariableSeries(Of Double)
        Private m_normgradientcolor1 As NormGradientColor
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
