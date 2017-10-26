Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    Public Class vb_Chaos_AC_Oscillator
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ao.Value = (PublicFunctions.Average(Me.m_avg_price, 5) - PublicFunctions.Average(Me.m_avg_price, &H22))
            Me.m_ac.Value = (Me.m_ao.Value - PublicFunctions.Average(Me.m_ao, 5))
            If (MyBase.Bars.CurrentBar >= 5) Then
                If PublicFunctions.DoubleGreater(Me.m_ac.Value, Me.m_ac.Item(1)) Then
                    Me.Plot1.Set(0, Me.m_ac.Value)
                Else
                    Me.Plot2.Set(0, Me.m_ac.Value)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ao = New VariableSeries(Of Double)(Me)
            Me.m_ac = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("+AC", EPlotShapes.Histogram, Color.Lime, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("-AC", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_avg_price = New Lambda(Of Double)(Function(_bb) ((MyBase.Bars.High.Item(_bb) + MyBase.Bars.Low.Item(_bb)) / 2))
        End Sub


        ' Fields
        Private m_ac As VariableSeries(Of Double)
        Private m_ao As VariableSeries(Of Double)
        Private m_avg_price As ISeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
