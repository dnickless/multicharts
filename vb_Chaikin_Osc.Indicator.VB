Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Chaikin_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Red
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.alertlength = 14
            Me.slowlength = 10
            Me.fastlength = 3
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_chaikinoscval.Value = Me.m_chaikinosc1.Item(0)
            Me.Plot1.Set(0, Me.m_chaikinoscval.Value)
            Me.Plot2.Set(0, 0)
            If ((Color.Empty <> Me.upcolor) AndAlso (Color.Empty <> Me.dncolor)) Then
                Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
                If (Me.m_applicationtype = 1) Then
                    Me.Plot1.Colors.Item(0) = m_colorlevel
                ElseIf (Me.m_applicationtype > 1) Then
                    Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                    Me.Plot1.BGColor = m_colorlevel
                End If
            End If
            If ((MyBase.Bars.Close.LowestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_chaikinoscval.LowestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0  - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_chaikinoscval.HighestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_chaikinosc1 = New [Function].ChaikinOsc(Me)
            Me.m_normgradientcolor1 = New [Function].NormGradientColor(Me)
            Me.m_applicationtype = 0
            Me.m_chaikinoscval = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ChaikinOsc", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_chaikinosc1.anyvol = MyBase.Bars.TrueVolume()
            Me.m_chaikinosc1.shortlength = Me.fastlength
            Me.m_chaikinosc1.longlength = Me.slowlength
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_chaikinoscval
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
            Me.m_applicationtype = CInt(MyBase.Environment.ApplicationCode)
        End Sub


        ' Properties
        <Input> _
        Public Property alertlength As Integer

        <Input()> _
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
        Private m_chaikinosc1 As [Function].ChaikinOsc
        Private m_chaikinoscval As VariableSeries(Of Double)
        Private m_normgradientcolor1 As [Function].NormGradientColor
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
