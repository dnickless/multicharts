Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_MornDoji_EveDoji
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.percent = 5
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_morndoji_evedoji1.Call
            If (Me.m_omorningdojistar.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("MorningDojiStar", New Object(0 - 1) {})
            ElseIf (Me.m_oeveningdojistar.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("EveningDojiStar", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_morndoji_evedoji1 = New [Function].C_MornDoji_EveDoji(Me)
            Me.m_omorningdojistar = New VariableObject(Of Integer)(Me)
            Me.m_oeveningdojistar = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MornDoji", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("EveDoji", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_morndoji_evedoji1.length = Me.length
            Me.m_c_morndoji_evedoji1.percent = Me.percent
            Me.m_c_morndoji_evedoji1.omorningdojistar = Me.m_omorningdojistar
            Me.m_c_morndoji_evedoji1.oeveningdojistar = Me.m_oeveningdojistar
            Me.m_omorningdojistar.DefaultValue = 0
            Me.m_oeveningdojistar.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property percent As Double


        ' Fields
        Private m_c_morndoji_evedoji1 As [Function].C_MornDoji_EveDoji
        Private m_oeveningdojistar As VariableObject(Of Integer)
        Private m_omorningdojistar As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
