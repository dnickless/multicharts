Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Momentum
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.gridforegroundcolor = Color.Black
            Me.dncolor = Color.Red
            Me.upcolor = Color.Yellow
            Me.colornormlength = 14
            Me.length = 12
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mom.Value = Me.m_price.Momentum(Me.length)
            Dim m_accel As Double = Me.m_mom.Momentum(1)
            Me.Plot1.Set(0, Me.m_mom.Value)
            Me.Plot2.Set(0, 0)
            If (Not Me.upcolor.IsEmpty AndAlso Not Me.dncolor.IsEmpty) Then
                Dim m_colorlevel As Color = Me.m_normgradientcolor1.Item(0)
                If (Me.m_applicationtype = 1) Then
                    Me.Plot1.Colors.Item(0) = m_colorlevel
                ElseIf (Me.m_applicationtype > 1) Then
                    Me.Plot1.Colors.Item(0) = Me.gridforegroundcolor
                    Me.Plot1.BGColor = m_colorlevel
                End If
            End If
            If (PublicFunctions.DoubleGreater(Me.m_mom.Value, 0) AndAlso PublicFunctions.DoubleGreater(m_accel, 0)) Then
                MyBase.Alerts.Alert("Indicator positive and increasing", New Object(0  - 1) {})
            ElseIf (PublicFunctions.DoubleLess(Me.m_mom.Value, 0) AndAlso PublicFunctions.DoubleLess(m_accel, 0)) Then
                MyBase.Alerts.Alert("Indicator negative and decreasing", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_normgradientcolor1 = New NormGradientColor(Me)
            Me.m_mom = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vb_Momentum", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
            Me.m_normgradientcolor1.dataseriesvalue = Me.m_mom
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
        Private m_mom As VariableSeries(Of Double)
        Private m_normgradientcolor1 As NormGradientColor
        Private m_price As ISeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
