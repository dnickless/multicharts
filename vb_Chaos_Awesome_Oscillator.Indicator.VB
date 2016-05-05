Imports PowerLanguage
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Chaos_Awesome_Oscillator
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ao.Value = (PublicFunctions.Average(Me.m_avg_price, 5) - PublicFunctions.Average(Me.m_avg_price, &H22))
            Me.Plot3.Set(0, 0)
            If (MyBase.Bars.CurrentBar >= 1) Then
                If PublicFunctions.DoubleGreater(Me.m_ao.Value, Me.m_ao.Item(1)) Then
                    Me.Plot1.Set(0, Me.m_ao.Value)
                Else
                    Me.Plot2.Set(0, Me.m_ao.Value)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ao = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("+AO", EPlotShapes.Histogram, Color.LimeGreen, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("-AO", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Empty, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_avg_price = New Lambda(Of Double)(Function(_bb) ((MyBase.Bars.High.Item(_bb) + MyBase.Bars.Low.Item(_bb)) / 2))
        End Sub


        ' Fields
        Private m_ao As VariableSeries(Of Double)
        Private m_avg_price As ISeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
