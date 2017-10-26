Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Consecutive_Ups
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.consecutivebarsup = 3
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(Me.m_price.Item(0), Me.m_price.Item(1)) Then
                Me.m_value1.Value = (Me.m_value1.Value + 1)
            Else
                Me.m_value1.Value = 0
            End If
            If PublicFunctions.DoubleGreaterEquals(CDbl(Me.m_value1.Value), Me.consecutivebarsup) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_value1 = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ConsecUp", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property consecutivebarsup As Double


        ' Fields
        Private m_price As ISeries(Of Double)
        Private m_value1 As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
    End Class
End Namespace
