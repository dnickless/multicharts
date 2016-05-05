Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_PercentR_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.TriggerLong = 62
            Me.OverBought = 80
            Me.OverSold = 20
            Me.Length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mypercentr.Value = MyBase.Bars.PercentR(Me.Length)
            Me.m_avg.Value = PublicFunctions.Average(MyBase.Bars.Close, Me.Length)
            If PublicFunctions.DoubleLess(Me.m_mypercentr.Value, Me.OverSold) Then
                Me.m_setupl.Value = True
            ElseIf PublicFunctions.DoubleGreater(Me.m_mypercentr.Value, Me.OverBought) Then
                Me.m_setupl.Value = False
            End If
            If ((Me.m_setupl.Value AndAlso PublicFunctions.DoubleGreater(Me.m_avg.Value, Me.m_avg.Item(1))) AndAlso Me.CrossesOver(Me.m_mypercentr, Me.TriggerLong)) Then
                Me.m_PctRLE.Send
                Me.m_setupl.Value = False
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mypercentr = New VariableSeries(Of Double)(Me)
            Me.m_avg = New VariableSeries(Of Double)(Me)
            Me.m_PctRLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "PctRLE", EOrderAction.Buy))
            Me.m_setupl = New VariableObject(Of Boolean)(Me)
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property OverBought As Double

        <Input()> _
        Public Property OverSold As Double

        <Input()> _
        Public Property TriggerLong As Double


        ' Fields
        Private m_avg As VariableSeries(Of Double)
        Private m_mypercentr As VariableSeries(Of Double)
        Private m_PctRLE As IOrderMarket
        Private m_setupl As VariableObject(Of Boolean)
    End Class
End Namespace
