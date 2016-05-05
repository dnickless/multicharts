Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_HPI
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Cyan
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.alertlength = 14
            Me.smoothingfactor = 0.133
            Me.onecent = 100
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Bars.CurrentBar = 1) Then
                Me.m_applicationtype.Value = DirectCast(MyBase.Environment.ApplicationCode, Integer)
            End If
            Me.m_hpivalue.Value = (Me.m_hpi1.Item(0) * 1E-05)
            Me.Plot1.Set(0, Me.m_hpivalue.Value)
            Me.Plot2.Set(0, 0)
            If ((Color.Empty <> Me.upcolor) AndAlso (Color.Empty <> Me.dncolor)) Then
                Me.m_colorlevel = Me.m_normgradientcolor1.Item(0)
                If (Me.m_applicationtype.Value = 1) Then
                    Me.Plot1.Colors.Item(0) = Me.m_colorlevel
                ElseIf (Me.m_applicationtype.Value > 1) Then
                    Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                    Me.Plot1.BGColor = Me.m_colorlevel
                End If
            End If
            If ((MyBase.Bars.Close.LowestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_hpivalue.LowestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bullish divergence - new low not confirmed", New Object(0  - 1) {})
            ElseIf ((MyBase.Bars.Close.HighestBar(Me.alertlength) = 0) AndAlso PublicFunctions.DoubleGreater(CDbl(Me.m_hpivalue.HighestBar(Me.alertlength)), 0)) Then
                MyBase.Alerts.Alert("Bearish divergence - new high not confirmed", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_hpi1 = New [Function].HPI(Me)
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_applicationtype = New VariableObject(Of Integer)(Me)
            Me.m_hpivalue = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("HPI", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_hpi1.onecent = Me.onecent
            Me.m_hpi1.smfactor = Me.smoothingfactor
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_hpivalue
            Me.m_normgradientcolor1.crosseszero = True
            Me.m_normgradientcolor1.colornormlength = Me.colornormlength
            Me.m_normgradientcolor1.upcolor = Me.upcolor
            Me.m_normgradientcolor1.dncolor = Me.dncolor
            Me.m_applicationtype.DefaultValue = 0
            Me.m_hpivalue.DefaultValue = 0
            Me.m_colorlevel = Color.Empty
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

        <Input()> _
        Public Property onecent As Double

        <Input()> _
        Public Property smoothingfactor As Double

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_applicationtype As VariableObject(Of Integer)
        Private m_colorlevel As Color
        Private m_hpi1 As [Function].HPI
        Private m_hpivalue As VariableSeries(Of Double)
        Private m_normgradientcolor1 As NormGradientColor
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
