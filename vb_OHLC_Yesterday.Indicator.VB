Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_OHLC_Yesterday
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((resolution <> EResolution.Quarter) AndAlso ((EResolution.Week > resolution) OrElse (resolution > EResolution.Year))) Then
                If (Bars.Time.Item(0).Date <> Bars.Time.Item(1).Date) Then
                    Me.m_counter.Value = (Me.m_counter.Value + 1)
                    Me.m_yestopen.Value = Me.m_todaysopen.Value
                    Me.m_yesthigh.Value = Me.m_todayshigh.Value
                    Me.m_yestlow.Value = Me.m_todayslow.Value
                    Me.m_yestclose.Value = MyBase.Bars.Close.Item(1)
                    Me.m_todaysopen.Value = MyBase.Bars.Open.Item(0)
                    Me.m_todayshigh.Value = MyBase.Bars.High.Item(0)
                    Me.m_todayslow.Value = MyBase.Bars.Low.Item(0)
                Else
                    If PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_todayshigh.Value) Then
                        Me.m_todayshigh.Value = MyBase.Bars.High.Item(0)
                    End If
                    If PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_todayslow.Value) Then
                        Me.m_todayslow.Value = MyBase.Bars.Low.Item(0)
                    End If
                End If
                If (Me.m_counter.Value >= 2) Then
                    Me.Plot1.Set(0, Me.m_yestopen.Value)
                    Me.Plot2.Set(0, Me.m_yesthigh.Value)
                    Me.Plot3.Set(0, Me.m_yestlow.Value)
                    Me.Plot4.Set(0, Me.m_yestclose.Value)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_counter = New VariableObject(Of Integer)(Me)
            Me.m_yestopen = New VariableObject(Of Double)(Me)
            Me.m_todaysopen = New VariableObject(Of Double)(Me)
            Me.m_yesthigh = New VariableObject(Of Double)(Me)
            Me.m_todayshigh = New VariableObject(Of Double)(Me)
            Me.m_yestlow = New VariableObject(Of Double)(Me)
            Me.m_todayslow = New VariableObject(Of Double)(Me)
            Me.m_yestclose = New VariableObject(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("YestOpen", EPlotShapes.LeftTick, Color.Magenta, Color.Empty, 2, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("YestHigh", EPlotShapes.LeftTick, Color.Blue, Color.Empty, 2, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("YestLow", EPlotShapes.LeftTick, Color.Yellow, Color.Empty, 2, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("YestClose", EPlotShapes.LeftTick, Color.Cyan, Color.Empty, 2, 0, True))
        End Sub


        ' Fields
        Private m_counter As VariableObject(Of Integer)
        Private m_todayshigh As VariableObject(Of Double)
        Private m_todayslow As VariableObject(Of Double)
        Private m_todaysopen As VariableObject(Of Double)
        Private m_yestclose As VariableObject(Of Double)
        Private m_yesthigh As VariableObject(Of Double)
        Private m_yestlow As VariableObject(Of Double)
        Private m_yestopen As VariableObject(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
