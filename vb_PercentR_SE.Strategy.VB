Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_PercentR_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.TriggerShort = 38
            Me.OverBought = 80
            Me.OverSold = 20
            Me.Length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mypercentr.Value = MyBase.Bars.PercentR(Me.Length)
            Me.m_avg.Value = PublicFunctions.Average(MyBase.Bars.Close, Me.Length)
            If PublicFunctions.DoubleLess(Me.m_mypercentr.Value, Me.OverSold) Then
                Me.m_setups.Value = False
            ElseIf PublicFunctions.DoubleGreater(Me.m_mypercentr.Value, Me.OverBought) Then
                Me.m_setups.Value = True
            End If
            If ((Me.m_setups.Value AndAlso PublicFunctions.DoubleLess(Me.m_avg.Value, Me.m_avg.Item(1))) AndAlso Me.CrossesUnder(Me.m_mypercentr, Me.TriggerShort)) Then
                Me.m_PctRSE.Send
                Me.m_setups.Value = False
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mypercentr = New VariableSeries(Of Double)(Me)
            Me.m_avg = New VariableSeries(Of Double)(Me)
            Me.m_PctRSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "PctRSE", EOrderAction.SellShort))
            Me.m_setups = New VariableObject(Of Boolean)(Me)
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property OverBought As Double

        <Input()> _
        Public Property OverSold As Double

        <Input()> _
        Public Property TriggerShort As Double


        ' Fields
        Private m_avg As VariableSeries(Of Double)
        Private m_mypercentr As VariableSeries(Of Double)
        Private m_PctRSE As IOrderMarket
        Private m_setups As VariableObject(Of Boolean)
    End Class
End Namespace
