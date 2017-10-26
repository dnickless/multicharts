Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Weighted
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.m_avgwtd.Value = PublicFunctions.WAverageCustom(Me.price, Me.length)
                Me.Plot1.Set(Me.displace, Me.m_avgwtd.Value)
                If (Me.displace <= 0) Then
                    If ((PublicFunctions.DoubleGreater(Me.price.Item(0), Me.m_avgwtd.Value) AndAlso PublicFunctions.DoubleGreater(Me.m_avgwtd.Value, Me.m_avgwtd.Item(1))) AndAlso PublicFunctions.DoubleLessEquals(Me.m_avgwtd.Item(1), Me.m_avgwtd.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
                    ElseIf ((PublicFunctions.DoubleLess(Me.price.Item(0), Me.m_avgwtd.Value) AndAlso PublicFunctions.DoubleLess(Me.m_avgwtd.Value, Me.m_avgwtd.Item(1))) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_avgwtd.Item(1), Me.m_avgwtd.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_avgwtd = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("AvgWtd", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_avgwtd As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
