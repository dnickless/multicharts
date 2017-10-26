Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Typical_Price
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_typprice.Value = MyBase.Bars.TypicalPrice(0)
            Me.m_avg.Value = Me.m_averagefc1.Item(0)
            Me.Plot1.Set(0, Me.m_typprice.Value)
            Me.Plot2.Set(0, Me.m_avg.Value)
            If Me.CrossesOver(Me.m_typprice, Me.m_avg) Then
                MyBase.Alerts.Alert("Typ Price crossing over its average", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_typprice, Me.m_avg) Then
                MyBase.Alerts.Alert("Typ Price crossing under its average", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_averagefc1 = New [Function].AverageFC(Me)
            Me.m_typprice = New VariableSeries(Of Double)(Me)
            Me.m_avg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("TypPrice", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Avg", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_averagefc1.price = Me.m_typprice
            Me.m_averagefc1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_averagefc1 As [Function].AverageFC
        Private m_avg As VariableSeries(Of Double)
        Private m_typprice As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
