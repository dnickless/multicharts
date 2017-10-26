Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_ZigZag_Pcnt
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.m_retracepct = 5
            Me.m_linecolor = Color.Yellow
            Me.m_linewidth = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_newswingprice = Me.m_swinghigh1.Item(0)
            If (Me.m_newswingprice <> -1) Then
                If (PublicFunctions.DoubleLessEquals(Me.m_tldir.Value, 0) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_newswingprice, (Me.m_swingprice.Value * Me.m_retracefctrup))) Then
                    Me.m_saveswing.Value = True
                    Me.m_addtl.Value = True
                    Me.m_tldir.Value = 1
                ElseIf ((Me.m_tldir.Value = 1) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_newswingprice, Me.m_swingprice.Value)) Then
                    Me.m_saveswing.Value = True
                    Me.m_updatetl.Value = True
                End If
            Else
                Me.m_newswingprice = Me.m_swinglow1.Item(0)
                If (Me.m_newswingprice <> -1) Then
                    If (PublicFunctions.DoubleGreaterEquals(Me.m_tldir.Value, 0) AndAlso PublicFunctions.DoubleLessEquals(Me.m_newswingprice, (Me.m_swingprice.Value * Me.m_retracefctrdn))) Then
                        Me.m_saveswing.Value = True
                        Me.m_addtl.Value = True
                        Me.m_tldir.Value = -1
                    ElseIf ((Me.m_tldir.Value = -1) AndAlso PublicFunctions.DoubleLessEquals(Me.m_newswingprice, Me.m_swingprice.Value)) Then
                        Me.m_saveswing.Value = True
                        Me.m_updatetl.Value = True
                    End If
                End If
            End If
            If Me.m_saveswing.Value Then
                Me.m_swingprice.Value = Me.m_newswingprice
                Me.m_swingtime.Value = MyBase.Bars.Time.Item(1)
                Me.m_saveswing.Value = False
            End If
            If Me.m_addtl.Value Then
                Me.m_tlref.Value = MyBase.DrwTrendLine.Create(New ChartPoint(Me.m_swingtime.Value, Me.m_swingprice.Value), New ChartPoint(Me.m_swingtime.Item(1), Me.m_swingprice.Item(1)))
                Me.m_tlref.Value.ExtLeft = False
                Me.m_tlref.Value.ExtRight = False
                Me.m_tlref.Value.Size = CInt(Me.linewidth)
                Me.m_tlref.Value.Color = Me.m_linecolor
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
            Me.m_tldir = New VariableObject(Of Double)(Me)
            Me.m_saveswing = New VariableObject(Of Boolean)(Me)
            Me.m_addtl = New VariableObject(Of Boolean)(Me)
            Me.m_updatetl = New VariableObject(Of Boolean)(Me)
            Me.m_tlref = New VariableObject(Of ITrendLineObject)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
            Me.m_swinghigh1.instance = 1
            Me.m_swinghigh1.price = Me.price
            Me.m_swinghigh1.strength = 1
            Me.m_swinghigh1.length = 2
            Me.m_swinglow1.instance = 1
            Me.m_swinglow1.price = Me.price
            Me.m_swinglow1.strength = 1
            Me.m_swinglow1.length = 2
            Me.m_newswingprice = 0
            Me.m_swingprice.DefaultValue = Me.price.Item(0)
            Me.m_swingtime.DefaultValue = MyBase.Bars.Time.Item(0)
            Me.m_tldir.DefaultValue = 0
            Me.m_retracefctrup = (1 + (Me.retracepct * 0.01))
            Me.m_retracefctrdn = (1 - (Me.retracepct * 0.01))
            Me.m_saveswing.DefaultValue = False
            Me.m_addtl.DefaultValue = False
            Me.m_updatetl.DefaultValue = False
            Me.m_tlref.DefaultValue = Nothing
        End Sub


        ' Properties
        <Input> _
        Public Property linecolor As KnownColor
            Get
                Return Me.m_linecolor.ToKnownColor
            End Get
            Set(ByVal value As KnownColor)
                Me.m_linecolor = Color.FromKnownColor(value)
            End Set
        End Property

        <Input> _
        Public Property linewidth As Double
            Get
                Return Me.m_linewidth
            End Get
            Set(ByVal value As Double)
                Me.m_linewidth = value
            End Set
        End Property

        Private ReadOnly Property price As ISeries(Of Double)
            Get
                Return Me.m_price
            End Get
        End Property

        <Input> _
        Public Property retracepct As Double
            Get
                Return Me.m_retracepct
            End Get
            Set(ByVal value As Double)
                Me.m_retracepct = value
            End Set
        End Property


        ' Fields
        Private m_addtl As VariableObject(Of Boolean)
        Private m_linecolor As Color
        Private m_linewidth As Double
        Private m_newswingprice As Double
        Private m_price As ISeries(Of Double)
        Private m_retracefctrdn As Double
        Private m_retracefctrup As Double
        Private m_retracepct As Double
        Private m_saveswing As VariableObject(Of Boolean)
        Private m_swinghigh1 As SwingHigh
        Private m_swinglow1 As SwingLow
        Private m_swingprice As VariableSeries(Of Double)
        Private m_swingtime As VariableSeries(Of DateTime)
        Private m_tldir As VariableObject(Of Double)
        Private m_tlref As VariableObject(Of ITrendLineObject)
        Private m_updatetl As VariableObject(Of Boolean)
    End Class
End Namespace
