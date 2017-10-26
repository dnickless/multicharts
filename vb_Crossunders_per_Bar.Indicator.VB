Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Crossunders_per_Bar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.datanum = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_formula2 As Double = Me.formula2.Value
            If (MyBase.Bars.LastBarOnChart AndAlso (MyBase.Bars.Status <> EBarState.Close)) Then
                If (MyBase.Bars.CurrentBar > Me.m_mycurrentbar.Value) Then
                    Me.m_crossunders.Value = 0
                    Me.m_mycurrentbar.Value = MyBase.Bars.CurrentBar
                End If
                If Not If(Not PublicFunctions.DoubleLess(Me.formula1.Item(0), m_formula2), True, Not Me.m_lastabove.Value) Then
                    Me.m_crossunders.Value = (Me.m_crossunders.Value + 1)
                    Me.m_lastabove.Value = False
                ElseIf PublicFunctions.DoubleGreater(Me.formula1.Item(0), m_formula2) Then
                    Me.m_lastabove.Value = True
                End If
                Me.Plot1.Set(0, Me.m_crossunders.Value)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mycurrentbar = New VariableObject(Of Integer)(Me)
            Me.m_crossunders = New VariableObject(Of Double)(Me)
            Me.m_lastabove = New VariableObject(Of Boolean)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Crossunders", EPlotShapes.Histogram, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.formula1 = MyBase.Bars.Close
            Me.formula2 = New Lambda(Of Double)(Function(_bb) PublicFunctions.Average(MyBase.Bars.Close, 9, _bb))
        End Sub


        ' Properties
        <Input> _
        Public Property datanum As Double

        Private Property formula1 As ISeries(Of Double)

        Private Property formula2 As ISeries(Of Double)


        ' Fields
        Private m_crossunders As VariableObject(Of Double)
        Private m_lastabove As VariableObject(Of Boolean)
        Private m_mycurrentbar As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
    End Class
End Namespace
