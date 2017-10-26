Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Block_Trades_per_Bar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.datanum = 1
            Me.blocksize = 5000
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Bars.LastBarOnChart AndAlso (MyBase.Bars.Status <> EBarState.Close)) Then
                If (Not Me.m_barnum.Value = MyBase.Bars.CurrentBar) Then
                    Me.m_blocktrades.Value = 0
                    Me.m_barnum.Value = MyBase.Bars.CurrentBar
                End If
                If PublicFunctions.DoubleGreaterEquals(0, Me.blocksize) Then
                    Me.m_blocktrades.Value = (Me.m_blocktrades.Value + 1)
                End If
                Me.Plot1.Set(0, Me.m_blocktrades.Value)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_blocktrades = New VariableObject(Of Double)(Me)
            Me.m_barnum = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BlockTrds", EPlotShapes.Histogram, Color.Blue, Color.Empty, 1, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property blocksize As Double

        <Input()> _
        Public Property datanum As Double


        ' Fields
        Private m_barnum As VariableObject(Of Integer)
        Private m_blocktrades As VariableObject(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
