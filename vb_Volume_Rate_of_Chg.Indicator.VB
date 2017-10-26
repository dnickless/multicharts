Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volume_Rate_of_Chg
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Cyan
            Me.colornormlength = 14
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_volroc.Value = (MyBase.Bars.VolumeROC(Me.length) * 100)
            Me.Plot1.Set(Me.m_volroc.Value)
            Me.Plot2.Set(0)
            If ((Color.Empty <> Me.upcolor) AndAlso (Color.Empty <> Me.dncolor)) Then
                Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
                If (Me.m_applicationtype = 1) Then
                    Me.Plot1.Colors.Item(0) = m_colorlevel
                ElseIf (Me.m_applicationtype > 1) Then
                    Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                    Me.Plot1.BGColor = m_colorlevel
                End If
            End If
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_volroc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning positive", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_volroc, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning negative", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_volroc = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VolROC", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_volroc
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
        Public Property gridforegroundcolor As Color

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_applicationtype As Integer
        Private m_normgradientcolor1 As NormGradientColor
        Private m_volroc As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
