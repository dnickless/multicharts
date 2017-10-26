Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Formula_OHLC
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.plotbaseline2 = True
            Me.plotbaseline1 = True
            Me.baseline2 = 30
            Me.baseline1 = 70
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_formula As Double = Me.m_rsi1.Item(0)
            If MyBase.Environment.IsRealTimeCalc Then
                If (Not Me.m_barnum.Value = MyBase.Bars.CurrentBar) Then
                    Me.m_formulaopen.Value = m_formula
                    Me.m_formulahigh.Value = m_formula
                    Me.m_formulalow.Value = m_formula
                    Me.m_barnum.Value = MyBase.Bars.CurrentBar
                End If
                If PublicFunctions.DoubleGreater(m_formula, Me.m_formulahigh.Value) Then
                    Me.m_formulahigh.Value = m_formula
                End If
                If PublicFunctions.DoubleLess(m_formula, Me.m_formulalow.Value) Then
                    Me.m_formulalow.Value = m_formula
                End If
                Me.Plot1.Set(0, Me.m_formulaopen.Value)
                Me.Plot2.Set(0, Me.m_formulahigh.Value)
                Me.Plot3.Set(0, Me.m_formulalow.Value)
                Me.Plot4.Set(0, m_formula)
            End If
            If Me.plotbaseline1 Then
                Me.Plot5.Set(0, Me.baseline1)
            End If
            If Me.plotbaseline2 Then
                Me.Plot6.Set(0, Me.baseline2)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_rsi1 = New [Function].RSI(Me)
            Me.m_barnum = New VariableObject(Of Integer)(Me)
            Me.m_formulaopen = New VariableObject(Of Double)(Me)
            Me.m_formulahigh = New VariableObject(Of Double)(Me)
            Me.m_formulalow = New VariableObject(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("FormulaOpen", EPlotShapes.LeftTick, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("FormulaHigh", EPlotShapes.BarHigh, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("FormulaLow", EPlotShapes.BarLow, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("FormulaClose", EPlotShapes.RightTick, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot5 = MyBase.AddPlot(New PlotAttributes("Baseline1", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
            Me.Plot6 = MyBase.AddPlot(New PlotAttributes("Baseline2", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_rsi1.price = MyBase.Bars.Close
            Me.m_rsi1.length = 14
        End Sub


        ' Properties
        <Input> _
        Public Property baseline1 As Double

        <Input()> _
        Public Property baseline2 As Double

        <Input()> _
        Public Property plotbaseline1 As Boolean

        <Input()> _
        Public Property plotbaseline2 As Boolean


        ' Fields
        Private m_barnum As VariableObject(Of Integer)
        Private m_formulahigh As VariableObject(Of Double)
        Private m_formulalow As VariableObject(Of Double)
        Private m_formulaopen As VariableObject(Of Double)
        Private m_rsi1 As [Function].RSI
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
        Private Plot5 As IPlotObject
        Private Plot6 As IPlotObject
    End Class
End Namespace
