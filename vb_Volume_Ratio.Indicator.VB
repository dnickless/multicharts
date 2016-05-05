Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volume_Ratio
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Cyan
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.dnvolalertpct = -5
            Me.upvolalertpct = 5
            Me.avglength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((((resolution < EResolution.Day) OrElse (EResolution.Volume = resolution)) OrElse (EResolution.Second = resolution)) OrElse (resolution > EResolution.Quarter)) Then
                Dim m_totalticks As Double = (MyBase.Bars.UpTicks.Item(0) + MyBase.Bars.DownTicks.Item(0))
                If PublicFunctions.DoubleGreater(m_totalticks, 0) Then
                    Me.m_volratio.Value = ((100 * (MyBase.Bars.UpTicks.Item(0) - MyBase.Bars.DownTicks.Item(0))) / m_totalticks)
                Else
                    Me.m_volratio.Value = 0
                End If
                Me.m_volratioavg.Value = Me.m_xaverage1.Item(0)
                Me.Plot1.Set(0, Me.m_volratioavg.Value)
                Me.Plot2.Set(0, 0)
                Me.Plot1.Colors.Item(0) = Me.m_normgradientcolor1.Item(0)
                If Me.CrossesOver(Me.m_volratioavg, Me.upvolalertpct) Then
                    MyBase.Alerts.Alert("UpVol alert", New Object(0  - 1) {})
                ElseIf Me.CrossesUnder(Me.m_volratioavg, Me.dnvolalertpct) Then
                    MyBase.Alerts.Alert("DnVol alert", New Object(0  - 1) {})
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_xaverage1 = New XAverage(Me)
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_volratio = New VariableSeries(Of Double)(Me)
            Me.m_volratioavg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VolRatioAvg", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 7, 7, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_xaverage1.Price = Me.m_volratio
            Me.m_xaverage1.Length = Me.avglength
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_volratioavg
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
        End Sub


        ' Properties
        <Input> _
        Public Property avglength As Integer

        <Input()> _
        Public Property colornormlength As Integer

        <Input()> _
        Public Property dncolor As Color

        <Input()> _
        Public Property dnvolalertpct As Double

        <Input()> _
        Public Property upcolor As Color

        <Input()> _
        Public Property upvolalertpct As Double


        ' Fields
        Private m_normgradientcolor1 As NormGradientColor
        Private m_volratio As VariableSeries(Of Double)
        Private m_volratioavg As VariableSeries(Of Double)
        Private m_xaverage1 As XAverage
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
