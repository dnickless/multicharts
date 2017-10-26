Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Swing_Index
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.upcolor = Color.Yellow
            Me.dncolor = Color.Cyan
            Me.colornormlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_swingx.Value = MyBase.Bars.Calc(0)
            Me.Plot1.Set(0, Me.m_swingx.Value)
            Me.Plot2.Set(0, 0)
            If (Not Me.upcolor.IsEmpty AndAlso Not Me.dncolor.IsEmpty) Then
                Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
                If (Me.m_applicationtype = 1) Then
                    Me.Plot1.Colors.Item(0) = m_colorlevel
                ElseIf (Me.m_applicationtype > 1) Then
                    Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                    Me.Plot1.BGColor = m_colorlevel
                End If
            End If
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_swingx, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning positive", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_swingx, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning negative", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_swingx = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("SwingX", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_swingx
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
        Public Property upcolor As Color


        ' Fields
        Private m_applicationtype As Integer
        Private m_normgradientcolor1 As NormGradientColor
        Private m_swingx As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
