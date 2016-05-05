Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volume_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.slowlength = &H1C
            Me.fastlength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_volosc.Value = Me.m_volumeosc1.Item(0)
            Me.Plot1.Set(0, Me.m_volosc.Value)
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
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_volosc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning positive", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_volosc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning negative", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_volumeosc1 = New VolumeOsc(Me)
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_volosc = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VolOsc", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_volumeosc1.fastlength = Me.fastlength
            Me.m_volumeosc1.slowlength = Me.slowlength
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_volosc
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
        Private m_normgradientcolor1 As NormGradientColor
        Private m_volosc As VariableSeries(Of Double)
        Private m_volumeosc1 As VolumeOsc
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
