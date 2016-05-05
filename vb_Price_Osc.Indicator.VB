Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Price_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.slowlength = &H12
            Me.fastlength = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_priceosc.Value = Me.m_priceoscillator1.Item(0)
            Me.Plot1.Set(0, Me.m_priceosc.Value)
            Me.Plot2.Set(0, 0)
            Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
            If (Me.m_applicationtype = 1) Then
                Me.Plot1.Colors.Item(0) = m_colorlevel
            ElseIf (Me.m_applicationtype > 1) Then
                Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                Me.Plot1.BGColor = m_colorlevel
            End If
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_priceosc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning positive", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_priceosc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning negative", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_priceoscillator1 = New [Function].PriceOscillator(Me)
            Me.m_normgradientcolor1 = New [Function].NormGradientColor(Me)
            Me.m_priceosc = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("PriceOsc", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_priceoscillator1.price = MyBase.Bars.Close
            Me.m_priceoscillator1.fastlength = Me.fastlength
            Me.m_priceoscillator1.slowlength = Me.slowlength
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_priceosc
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
            Me.m_applicationtype = CInt(MyBase.Environment.ApplicationCode)
        End Sub


        ' Properties
        <Input> _
        Public Property colornormlength As Integer

        <Input()> _
        Public Property dncolor As Color

        <Input()> _
        Public Property fastlength As Integer

        <Input()> _
        Public Property gridforegroundcolor As Color

        <Input()> _
        Public Property slowlength As Integer

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_applicationtype As Integer
        Private m_normgradientcolor1 As [Function].NormGradientColor
        Private m_priceosc As VariableSeries(Of Double)
        Private m_priceoscillator1 As [Function].PriceOscillator
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
