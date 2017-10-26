Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_ShootingStar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.factor = 2
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_c_shootingstar1.Item(0) = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.Close.Item(0))
                MyBase.Alerts.Alert("ShootingStar", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_shootingstar1 = New [Function].C_ShootingStar(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ShStar", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_shootingstar1.length = Me.length
            Me.m_c_shootingstar1.factor = Me.factor
        End Sub


        ' Properties
        <Input> _
        Public Property factor As Double

        <Input()> _
        Public Property length As Integer


        ' Fields
        Private m_c_shootingstar1 As [Function].C_ShootingStar
        Private Plot1 As IPlotObject
    End Class
End Namespace
