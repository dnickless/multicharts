Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Pivot_Rev_Gen__Dn
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.plotcolor = Color.Cyan
            Me.linescolor = Color.Magenta
            Me.colorscheme = 1
            Me.drawlines = True
            Me.lrfactor = 1
            Me.maxrstren = 30
            Me.minrstren = 1
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_pivotreversalgen1.Item(0) = 1) Then
                Me.Plot1.Set(0, Me.m_opivotprice.Value)
                If (Me.colorscheme = 1) Then
                    Me.Plot1.Colors.Item(0) = Me.plotcolor
                ElseIf (Me.colorscheme = 2) Then
                    Me.Plot1.BGColor = Me.plotcolor
                End If
                MyBase.Alerts.Alert(String.Concat(New Object() {"Pivot reversal: Lo = ", Math.Round(Me.m_opivotprice.Value, 2), ", RS = ", Math.Round(Me.m_opivotrstren.Value, 0), ", LS = ", Math.Round(Me.m_opivotlstren.Value, 0)}), New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_pivotreversalgen1 = New [Function].PivotReversalGen(Me)
            Me.m_opivotprice = New VariableObject(Of Double)(Me)
            Me.m_opivotrstren = New VariableObject(Of Double)(Me)
            Me.m_opivotlstren = New VariableObject(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("PivRev_Dn", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_pivotreversalgen1.minrstren = Me.minrstren
            Me.m_pivotreversalgen1.maxrstren = Me.maxrstren
            Me.m_pivotreversalgen1.lrfactor = Me.lrfactor
            Me.m_pivotreversalgen1.hilo = -1
            Me.m_pivotreversalgen1.drawlines = Me.drawlines
            Me.m_pivotreversalgen1.linescolor = Me.linescolor
            Me.m_pivotreversalgen1.opivotprice = Me.m_opivotprice
            Me.m_pivotreversalgen1.opivotrstren = Me.m_opivotrstren
            Me.m_pivotreversalgen1.opivotlstren = Me.m_opivotlstren
        End Sub


        ' Properties
        <Input> _
        Public Property colorscheme As Integer

        <Input()> _
        Public Property drawlines As Boolean

        <Input()> _
        Public Property linescolor As Color

        <Input()> _
        Public Property lrfactor As Double

        <Input()> _
        Public Property maxrstren As Double

        <Input()> _
        Public Property minrstren As Double

        <Input()> _
        Public Property plotcolor As Color


        ' Fields
        Private m_opivotlstren As VariableObject(Of Double)
        Private m_opivotprice As VariableObject(Of Double)
        Private m_opivotrstren As VariableObject(Of Double)
        Private m_pivotreversalgen1 As [Function].PivotReversalGen
        Private Plot1 As IPlotObject
    End Class
End Namespace
