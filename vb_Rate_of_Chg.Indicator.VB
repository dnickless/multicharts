Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Rate_of_Chg
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Cyan
            Me.colornormlength = 14
            Me.alertlength = 14
            Me.roclength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_roc.Value = Me.price.RateOfChange(Me.roclength)
            Me.Plot1.Set(0, Me.m_roc.Value)
            Me.Plot2.Set(0, 0)
            Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
            If (MyBase.Environment.ApplicationCode = EApplicationCode.Charting) Then
                Me.Plot1.Colors.Item(0) = m_colorlevel
            ElseIf (MyBase.Environment.ApplicationCode > EApplicationCode.Charting) Then
                Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                Me.Plot1.BGColor = m_colorlevel
            End If
            If (Me.m_roc.HighestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at high", New Object(0  - 1) {})
            ElseIf (Me.m_roc.LowestBar(Me.alertlength) = 0) Then
                MyBase.Alerts.Alert("Indicator at low", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_roc = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ROC", EPlotShapes.Line, Color.Silver, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_roc
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer

        <Input()> _
        Public Property colornormlength As Integer

        <Input()> _
        Public Property dncolor As Color

        <Input()> _
        Public Property gridforegroundcolor As Color

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property roclength As Integer

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_normgradientcolor1 As NormGradientColor
        Private m_roc As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
