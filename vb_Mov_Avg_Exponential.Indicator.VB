Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Exponential
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_avgexp.Value = Me.m_xaverage1.Item(0)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_avgexp.Value)
                If (Me.displace <= 0) Then
                    If ((PublicFunctions.DoubleGreater(Me.price.Item(0), Me.m_avgexp.Value) AndAlso PublicFunctions.DoubleGreater(Me.m_avgexp.Value, Me.m_avgexp.Item(1))) AndAlso PublicFunctions.DoubleLessEquals(Me.m_avgexp.Item(1), Me.m_avgexp.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
                    ElseIf ((PublicFunctions.DoubleLess(Me.price.Item(0), Me.m_avgexp.Value) AndAlso PublicFunctions.DoubleLess(Me.m_avgexp.Value, Me.m_avgexp.Item(1))) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_avgexp.Item(1), Me.m_avgexp.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_xaverage1 = New XAverage(Me)
            Me.m_avgexp = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("AvgExp", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_xaverage1.Price = Me.price
            Me.m_xaverage1.Length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_avgexp As VariableSeries(Of Double)
        Private m_xaverage1 As XAverage
        Private Plot1 As IPlotObject
    End Class
End Namespace
