Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Day_OpenHiLo_RT
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.plotopen = True
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Environment.IsRealTimeCalc Then
                Dim m_dhigh As Double = MyBase.Bars.StatusLine.High
                Dim m_dlow As Double = MyBase.Bars.StatusLine.Low
                Me.Plot1.Set(0, m_dhigh)
                Me.Plot2.Set(0, m_dlow)
                If Me.plotopen Then
                    Dim m_dopen As Double = MyBase.Bars.StatusLine.Open
                    Me.Plot3.Set(0, m_dopen)
                End If
                If Me.plotprevclose Then
                    Dim m_yestclose As Double = MyBase.Bars.StatusLine.PrevClose
                    Me.Plot4.Set(0, m_yestclose)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("High", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Low", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Open", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("YestClose", EPlotShapes.Line, Color.Silver, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property plotopen As Boolean

        <Input()> _
        Public Property plotprevclose As Boolean


        ' Fields
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
