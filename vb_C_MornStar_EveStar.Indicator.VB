Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_MornStar_EveStar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_mornstar_evestar1.Call
            If (Me.m_omorningstar.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("MorningStar", New Object(0 - 1) {})
            ElseIf (Me.m_oeveningstar.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("EveningStar", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_mornstar_evestar1 = New [Function].C_MornStar_EveStar(Me)
            Me.m_omorningstar = New VariableObject(Of Integer)(Me)
            Me.m_oeveningstar = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MornStar", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("EveStar", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_mornstar_evestar1.length = Me.length
            Me.m_c_mornstar_evestar1.omorningstar = Me.m_omorningstar
            Me.m_c_mornstar_evestar1.oeveningstar = Me.m_oeveningstar
            Me.m_omorningstar.DefaultValue = 0
            Me.m_oeveningstar.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_c_mornstar_evestar1 As [Function].C_MornStar_EveStar
        Private m_oeveningstar As VariableObject(Of Integer)
        Private m_omorningstar As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
