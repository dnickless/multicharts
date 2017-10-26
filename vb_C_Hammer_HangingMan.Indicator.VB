Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_Hammer_HangingMan
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.factor = 2
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_hammer_hangingman1.Call
            If (Me.m_ohammer.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("Hammer", New Object(0 - 1) {})
            ElseIf (Me.m_ohangingman.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("HangingMan", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_hammer_hangingman1 = New [Function].C_Hammer_HangingMan(Me)
            Me.m_ohammer = New VariableObject(Of Integer)(Me)
            Me.m_ohangingman = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Hammer", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("HangMan", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_hammer_hangingman1.length = Me.length
            Me.m_c_hammer_hangingman1.factor = Me.factor
            Me.m_c_hammer_hangingman1.ohammer = Me.m_ohammer
            Me.m_c_hammer_hangingman1.ohangingman = Me.m_ohangingman
            Me.m_ohammer.DefaultValue = 0
            Me.m_ohangingman.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property factor As Double

        <Input()> _
        Public Property length As Integer


        ' Fields
        Private m_c_hammer_hangingman1 As [Function].C_Hammer_HangingMan
        Private m_ohammer As VariableObject(Of Integer)
        Private m_ohangingman As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
