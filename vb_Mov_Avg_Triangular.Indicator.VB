Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Triangular
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_avgtri.Value = Me.m_triaverage1.Item(0)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_avgtri.Value)
                If (Me.displace <= 0) Then
                    If ((PublicFunctions.DoubleGreater(Me.price.Item(0), Me.m_avgtri.Value) AndAlso PublicFunctions.DoubleGreater(Me.m_avgtri.Value, Me.m_avgtri.Item(1))) AndAlso PublicFunctions.DoubleLessEquals(Me.m_avgtri.Item(1), Me.m_avgtri.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
                    ElseIf ((PublicFunctions.DoubleLess(Me.price.Item(0), Me.m_avgtri.Value) AndAlso PublicFunctions.DoubleLess(Me.m_avgtri.Value, Me.m_avgtri.Item(1))) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_avgtri.Item(1), Me.m_avgtri.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_triaverage1 = New TriAverage(Me)
            Me.m_avgtri = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("AvgTri", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_triaverage1.price = Me.price
            Me.m_triaverage1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_avgtri As VariableSeries(Of Double)
        Private m_triaverage1 As TriAverage
        Private Plot1 As IPlotObject
    End Class
End Namespace
