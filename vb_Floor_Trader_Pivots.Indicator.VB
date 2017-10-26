Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Floor_Trader_Pivots
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.plot_5or7 = 5
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((resolution <> EResolution.Quarter) AndAlso ((EResolution.Week > resolution) OrElse (resolution > EResolution.Year))) Then

                If (Bars.Time.Item(0).Date <> Bars.Time.Item(1).Date) Then
                    Me.m_counter.Value = (Me.m_counter.Value + 1)
                    Dim m_yesthigh As Double = Me.m_todayshigh.Value
                    Dim m_yestlow As Double = Me.m_todayslow.Value
                    Dim m_yestclose As Double = MyBase.Bars.Close.Item(1)
                    Me.m_todayshigh.Value = MyBase.Bars.High.Item(0)
                    Me.m_todayslow.Value = MyBase.Bars.Low.Item(0)
                    Me.m_pp.Value = (((m_yesthigh + m_yestlow) + m_yestclose) / 3)
                    Me.m_r1.Value = ((Me.m_pp.Value * 2) - m_yestlow)
                    Me.m_r2.Value = ((Me.m_pp.Value + m_yesthigh) - m_yestlow)
                    Me.m_r3.Value = ((Me.m_r2.Value + m_yesthigh) - m_yestlow)
                    Me.m_s1.Value = ((Me.m_pp.Value * 2) - m_yesthigh)
                    Me.m_s2.Value = ((Me.m_pp.Value - m_yesthigh) + m_yestlow)
                    Me.m_s3.Value = ((Me.m_s2.Value - m_yesthigh) + m_yestlow)
                Else
                    If PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_todayshigh.Value) Then
                        Me.m_todayshigh.Value = MyBase.Bars.High.Item(0)
                    End If
                    If PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_todayslow.Value) Then
                        Me.m_todayslow.Value = MyBase.Bars.Low.Item(0)
                    End If
                End If
                If (Me.m_counter.Value >= 2) Then
                    If (Me.plot_5or7 = 7) Then
                        Me.Plot1.Set(0, Me.m_r3.Value)
                    End If
                    Me.Plot2.Set(0, Me.m_r2.Value)
                    Me.Plot3.Set(0, Me.m_r1.Value)
                    Me.Plot4.Set(0, Me.m_pp.Value)
                    Me.Plot5.Set(0, Me.m_s1.Value)
                    Me.Plot6.Set(0, Me.m_s2.Value)
                    If (Me.plot_5or7 = 7) Then
                        Me.Plot7.Set(0, Me.m_s3.Value)
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_s1 = New VariableObject(Of Double)(Me)
            Me.m_s2 = New VariableObject(Of Double)(Me)
            Me.m_s3 = New VariableObject(Of Double)(Me)
            Me.m_r1 = New VariableObject(Of Double)(Me)
            Me.m_r2 = New VariableObject(Of Double)(Me)
            Me.m_r3 = New VariableObject(Of Double)(Me)
            Me.m_pp = New VariableObject(Of Double)(Me)
            Me.m_todayshigh = New VariableObject(Of Double)(Me)
            Me.m_todayslow = New VariableObject(Of Double)(Me)
            Me.m_counter = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("R3", EPlotShapes.LeftTick, Color.Magenta, Color.Empty, 2, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("R2", EPlotShapes.LeftTick, Color.Blue, Color.Empty, 2, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("R1", EPlotShapes.LeftTick, Color.Yellow, Color.Empty, 2, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("PP", EPlotShapes.LeftTick, Color.Cyan, Color.Empty, 2, 0, True))
            Me.Plot5 = MyBase.AddPlot(New PlotAttributes("S1", EPlotShapes.LeftTick, Color.Yellow, Color.Empty, 2, 0, True))
            Me.Plot6 = MyBase.AddPlot(New PlotAttributes("S2", EPlotShapes.LeftTick, Color.Blue, Color.Empty, 2, 0, True))
            Me.Plot7 = MyBase.AddPlot(New PlotAttributes("S3", EPlotShapes.LeftTick, Color.Magenta, Color.Empty, 2, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property plot_5or7 As Integer


        ' Fields
        Private m_counter As VariableObject(Of Integer)
        Private m_pp As VariableObject(Of Double)
        Private m_r1 As VariableObject(Of Double)
        Private m_r2 As VariableObject(Of Double)
        Private m_r3 As VariableObject(Of Double)
        Private m_s1 As VariableObject(Of Double)
        Private m_s2 As VariableObject(Of Double)
        Private m_s3 As VariableObject(Of Double)
        Private m_todayshigh As VariableObject(Of Double)
        Private m_todayslow As VariableObject(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
        Private Plot5 As IPlotObject
        Private Plot6 As IPlotObject
        Private Plot7 As IPlotObject
    End Class
End Namespace
