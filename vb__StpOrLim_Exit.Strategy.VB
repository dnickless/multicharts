Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb__StpOrLim_Exit
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.ExitQuantity = 1000
            Me.TriggerPrice = 100
            Me.SellOrCover = True
            Me.StopOrLimit = True
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ExitOrder.Send(Me.TriggerPrice, Me.ExitQuantity)
        End Sub

        Protected Overrides Sub Create()
            Me.m_SellStop = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Sell", EOrderAction.Sell, OrderExit.FromAll))
            Me.m_SellLimit = MyBase.OrderCreator.Limit(New SOrderParameters(Contracts.Default, "Sell#1", EOrderAction.Sell, OrderExit.FromAll))
            Me.m_CoverStop = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Cover", EOrderAction.BuyToCover, OrderExit.FromAll))
            Me.m_CoverLimit = MyBase.OrderCreator.Limit(New SOrderParameters(Contracts.Default, "Cover#1", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_ExitOrder = Nothing
            If (Me.SellOrCover AndAlso Me.StopOrLimit) Then
                Me.m_ExitOrder = Me.m_SellStop
            ElseIf Not (Not Me.SellOrCover OrElse Me.StopOrLimit) Then
                Me.m_ExitOrder = Me.m_SellLimit
            ElseIf Not (Me.SellOrCover OrElse Not Me.StopOrLimit) Then
                Me.m_ExitOrder = Me.m_CoverStop
            ElseIf Not (Me.SellOrCover OrElse Me.StopOrLimit) Then
                Me.m_ExitOrder = Me.m_CoverLimit
            End If
            If (Me.m_ExitOrder Is Nothing) Then
                Throw New ApplicationException("Invalid Parameter")
            End If
        End Sub


        ' Properties
        <Input> _
        Public Property ExitQuantity As Integer

        <Input()> _
        Public Property SellOrCover As Boolean

        <Input()> _
        Public Property StopOrLimit As Boolean

        <Input()> _
        Public Property TriggerPrice As Double


        ' Fields
        Private m_CoverLimit As IOrderPriced
        Private m_CoverStop As IOrderPriced
        Private m_ExitOrder As IOrderPriced
        Private m_SellLimit As IOrderPriced
        Private m_SellStop As IOrderPriced
    End Class
End Namespace
