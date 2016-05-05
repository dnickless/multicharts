Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Parabolic_SAR
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.afstep = 0.02
            Me.aflimit = 0.2
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_parabolicsar1.Call
            Me.Plot1.Set(0, Me.m_oparcl.Value)
            If (Me.m_otransition.Value = 1) Then
                MyBase.Alerts.Alert("Bullish reversal", New Object(0 - 1) {})
            ElseIf (Me.m_otransition.Value = -1) Then
                MyBase.Alerts.Alert("Bearish reversal", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_parabolicsar1 = New ParabolicSAR(Me)
            Me.m_oparcl = New VariableObject(Of Double)(Me)
            Me.m_oparop = New VariableObject(Of Double)(Me)
            Me.m_oposition = New VariableObject(Of Integer)(Me)
            Me.m_otransition = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ParCl", EPlotShapes.Point, Color.Blue, Color.Empty, 1, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_parabolicsar1.afstep = Me.afstep
            Me.m_parabolicsar1.aflimit = Me.aflimit
            Me.m_parabolicsar1.oparcl = Me.m_oparcl
            Me.m_parabolicsar1.oparop = Me.m_oparop
            Me.m_parabolicsar1.oposition = Me.m_oposition
            Me.m_parabolicsar1.otransition = Me.m_otransition
        End Sub


        ' Properties
        <Input> _
        Public Property aflimit As Double

        <Input()> _
        Public Property afstep As Double


        ' Fields
        Private m_oparcl As VariableObject(Of Double)
        Private m_oparop As VariableObject(Of Double)
        Private m_oposition As VariableObject(Of Integer)
        Private m_otransition As VariableObject(Of Integer)
        Private m_parabolicsar1 As ParabolicSAR
        Private Plot1 As IPlotObject
    End Class
End Namespace
