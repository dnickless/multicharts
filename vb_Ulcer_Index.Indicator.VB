Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Ulcer_Index
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.safelevel = 5
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ulcerx.Value = MyBase.Bars.Close.UlcerIndex(Me.length)
            Me.Plot1.Set(0, Me.m_ulcerx.Value)
            Me.Plot2.Set(0, Me.safelevel)
            If Me.CrossesOver(Me.m_ulcerx, Me.safelevel) Then
                MyBase.Alerts.Alert("Indicator crossing over Safe Level", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ulcerx = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UlcerX", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("SafeLevel", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property safelevel As Double


        ' Fields
        Private m_ulcerx As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
