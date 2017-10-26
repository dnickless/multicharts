Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Mass_Index
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.trigger = 26.5
            Me.setup = 27
            Me.summationlength = &H19
            Me.smoothinglength = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_massx.Value = Me.m_massindex1.Item(0)
            Me.Plot1.Set(0, Me.m_massx.Value)
            Me.Plot2.Set(0, Me.setup)
            Me.Plot3.Set(0, Me.trigger)
            If Me.CrossesOver(Me.m_massx, Me.setup) Then
                Me.m_setupstatus.Value = True
            ElseIf (Me.m_setupstatus.Value AndAlso Me.CrossesUnder(Me.m_massx, Me.trigger)) Then
                MyBase.Alerts.Alert("Reversal alert", New Object(0  - 1) {})
                Me.m_setupstatus.Value = False
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_massindex1 = New [Function].MassIndex(Me)
            Me.m_massx = New VariableSeries(Of Double)(Me)
            Me.m_setupstatus = New VariableObject(Of Boolean)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("MassX", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Setup", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Trigger", EPlotShapes.Line, Color.Lime, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_massindex1.smoothinglength = Me.smoothinglength
            Me.m_massindex1.summationlength = Me.summationlength
        End Sub


        ' Properties
        <Input> _
        Public Property setup As Double

        <Input()> _
        Public Property smoothinglength As Integer

        <Input()> _
        Public Property summationlength As Integer

        <Input()> _
        Public Property trigger As Double


        ' Fields
        Private m_massindex1 As [Function].MassIndex
        Private m_massx As VariableSeries(Of Double)
        Private m_setupstatus As VariableObject(Of Boolean)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
