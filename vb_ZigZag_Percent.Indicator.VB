Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_ZigZag_Percent
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.linewidth = 1
            Me.linecolor = Color.Yellow
            Me.retracepct = 5
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_newswingprice As Double = Me.m_swinghigh1.Item(0)
            If (m_newswingprice <> -1) Then
                If (PublicFunctions.DoubleLessEquals(CDbl(Me.m_tldir.Value), 0) AndAlso PublicFunctions.DoubleGreaterEquals(m_newswingprice, (Me.m_swingprice.Value * Me.m_retracefctrup))) Then
                    Me.m_saveswing.Value = True
                    Me.m_addtl.Value = True
                    Me.m_tldir.Value = 1
                ElseIf ((Me.m_tldir.Value = 1) AndAlso PublicFunctions.DoubleGreaterEquals(m_newswingprice, Me.m_swingprice.Value)) Then
                    Me.m_saveswing.Value = True
                    Me.m_updatetl.Value = True
                End If
            Else
                m_newswingprice = Me.m_swinglow1.Item(0)
                If (m_newswingprice <> -1) Then
                    If (PublicFunctions.DoubleGreaterEquals(CDbl(Me.m_tldir.Value), 0) AndAlso PublicFunctions.DoubleLessEquals(m_newswingprice, (Me.m_swingprice.Value * Me.m_retracefctrdn))) Then
                        Me.m_saveswing.Value = True
                        Me.m_addtl.Value = True
                        Me.m_tldir.Value = -1
                    ElseIf ((Me.m_tldir.Value = -1) AndAlso PublicFunctions.DoubleLessEquals(m_newswingprice, Me.m_swingprice.Value)) Then
                        Me.m_saveswing.Value = True
                        Me.m_updatetl.Value = True
                    End If
                End If
            End If
            If Me.m_saveswing.Value Then
                Me.m_swingprice.Value = m_newswingprice
                Me.m_swingtime.Value = MyBase.Bars.Time.Item(1)
                Me.m_saveswing.Value = False
            End If
            If Me.m_addtl.Value Then
                Me.m_tlref.Value = MyBase.DrwTrendLine.Create(New ChartPoint(Me.m_swingtime.Value, Me.m_swingprice.Value), New ChartPoint(Me.m_swingtime.Item(1), Me.m_swingprice.Item(1)))
                Me.m_tlref.Value.ExtLeft = False
                Me.m_tlref.Value.ExtRight = False
                Me.m_tlref.Value.Size = CInt(Me.linewidth)
                Me.m_tlref.Value.Color = Me.linecolor
                Me.m_addtl.Value = False
            ElseIf Me.m_updatetl.Value Then
                Me.m_tlref.Value.End = New ChartPoint(Me.m_swingtime.Value, Me.m_swingprice.Value)
                Me.m_updatetl.Value = False
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_swinghigh1 = New SwingHigh(Me)
            Me.m_swinglow1 = New SwingLow(Me)
            Me.m_swingprice = New VariableSeries(Of Double)(Me)
            Me.m_swingtime = New VariableSeries(Of DateTime)(Me)
            Me.m_tldir = New VariableObject(Of Integer)(Me)
            Me.m_saveswing = New VariableObject(Of Boolean)(Me)
            Me.m_addtl = New VariableObject(Of Boolean)(Me)
            Me.m_updatetl = New VariableObject(Of Boolean)(Me)
            Me.m_tlref = New VariableObject(Of ITrendLineObject)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_swinghigh1.instance = 1
            Me.m_swinghigh1.price = Me.price
            Me.m_swinghigh1.strength = 1
            Me.m_swinghigh1.length = 2
            Me.m_swinglow1.instance = 1
            Me.m_swinglow1.price = Me.price
            Me.m_swinglow1.strength = 1
            Me.m_swinglow1.length = 2
            Me.m_swingprice.DefaultValue = Me.price.Item(0)
            Me.m_swingtime.DefaultValue = MyBase.Bars.Time.Item(0)
            Me.m_tldir.DefaultValue = 0
            Me.m_retracefctrup = (1 + (Me.retracepct * 0.01))
            Me.m_retracefctrdn = (1 - (Me.retracepct * 0.01))
        End Sub


        ' Properties
        <Input> _
        Public Property linecolor As Color

        <Input()> _
        Public Property linewidth As Double

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property retracepct As Double


        ' Fields
        Private m_addtl As VariableObject(Of Boolean)
        Private m_retracefctrdn As Double
        Private m_retracefctrup As Double
        Private m_saveswing As VariableObject(Of Boolean)
        Private m_swinghigh1 As SwingHigh
        Private m_swinglow1 As SwingLow
        Private m_swingprice As VariableSeries(Of Double)
        Private m_swingtime As VariableSeries(Of DateTime)
        Private m_tldir As VariableObject(Of Integer)
        Private m_tlref As VariableObject(Of ITrendLineObject)
        Private m_updatetl As VariableObject(Of Boolean)
    End Class
End Namespace
