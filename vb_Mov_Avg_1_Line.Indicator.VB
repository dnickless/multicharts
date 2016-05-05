Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_1_Line
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_avg.Value = Me.m_averagefc1.Item(0)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_avg.Value)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.price, Me.m_avg) Then
                        MyBase.Alerts.Alert("Price crossing over average", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.price, Me.m_avg) Then
                        MyBase.Alerts.Alert("Price crossing under average", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New AverageFC(Me)
            Me.m_avg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Avg", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_averagefc1.price = Me.price
            Me.m_averagefc1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        Private Property price As ISeries(Of Double)


        ' Fields
        Private m_averagefc1 As AverageFC
        Private m_avg As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
