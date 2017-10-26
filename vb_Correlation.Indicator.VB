Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Correlation
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.negcorralert = -0.7
            Me.poscorralert = 0.7
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_correlval.Value = [Function].Correlation.CalcCorrelation(Me.indepdata, Me.depdata, Me.length)
            Me.Plot1.Set(0, Me.m_correlval.Value)
            Me.Plot2.Set(0, Me.poscorralert)
            Me.Plot3.Set(0, Me.negcorralert)
            If Me.CrossesOver(Me.m_correlval, Me.poscorralert) Then
                MyBase.Alerts.Alert("Positive correlation alert", New Object(0  - 1) {})
            ElseIf Me.CrossesUnder(Me.m_correlval, Me.negcorralert) Then
                MyBase.Alerts.Alert("Negative correlation alert", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_correlval = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Correl", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("PosCorrAlert", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("NegCorrAlert", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.indepdata = MyBase.Bars.Close
            Me.depdata = MyBase.BarsOfData(2).Close
        End Sub


        ' Properties
        Private Property depdata As ISeries(Of Double)

        Private Property indepdata As ISeries(Of Double)

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property negcorralert As Double

        <Input()> _
        Public Property poscorralert As Double


        ' Fields
        Private m_correlval As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
