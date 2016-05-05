Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volume_Avg
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.downcolor = Color.Red
            Me.upcolor = Color.Cyan
            Me.alertpct = 50
            Me.avglength = 50
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_vvol.Value = Me.Volume.Item(0)
            Me.m_avgvvol.Value = Me.m_averagefc1.Item(0)
            Me.Plot1.Set(0, Me.m_vvol.Value)
            Me.Plot2.Set(0, Me.m_avgvvol.Value)
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_vvol, ISeries(Of Double)), CDbl((Me.m_avgvvol.Value * Me.m_alertfactor))) Then
                MyBase.Alerts.Alert(("Volume breaking through " & Me.m_alertstr & "% above its avg"), New Object(0 - 1) {})
            End If
            If PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(0), MyBase.Bars.Close.Item(1)) Then
                Me.Plot1.Colors.Item(0) = Me.upcolor
            ElseIf PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), MyBase.Bars.Close.Item(1)) Then
                Me.Plot1.Colors.Item(0) = Me.downcolor
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_vvol = New VariableSeries(Of Double)(Me)
            Me.m_avgvvol = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Vol", EPlotShapes.Histogram, Color.White, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("VolAvg", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Volume = MyBase.Bars.TrueVolume()
            Me.m_averagefc1.price = Me.Volume
            Me.m_averagefc1.length = Me.avglength
            Me.m_alertfactor = (1 + (Me.alertpct * 0.01))
            Me.m_alertstr = Math.Round(Me.alertpct, 2).ToString
        End Sub


        ' Properties
        <Input> _
        Public Property alertpct As Double

        <Input()> _
        Public Property avglength As Integer

        <Input()> _
        Public Property downcolor As Color

        <Input()> _
        Public Property upcolor As Color

        Private Property Volume As ISeries(Of Double)


        ' Fields
        Private m_alertfactor As Double
        Private m_alertstr As String
        Private m_averagefc1 As [Function].AverageFC
        Private m_avgvvol As VariableSeries(Of Double)
        Private m_vvol As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
