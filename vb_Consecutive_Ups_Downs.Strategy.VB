Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Consecutive_Ups_Downs
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.ConsecutiveBarsUp = 3
            Me.ConsecutiveBarsDown = 3
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(Me.price.Item(0), Me.price.Item(1)) Then
                Me.m_ups_cnt.Value = (Me.m_ups_cnt.Value + 1)
            Else
                Me.m_ups_cnt.Value = 0
            End If
            If PublicFunctions.DoubleLess(Me.price.Item(0), Me.price.Item(1)) Then
                Me.m_downs_cnt.Value = (Me.m_downs_cnt.Value + 1)
            Else
                Me.m_downs_cnt.Value = 0
            End If
            If (Me.m_ups_cnt.Value >= Me.ConsecutiveBarsUp) Then
                Me.m_ConsUpLE.Send
            End If
            If (Me.m_downs_cnt.Value >= Me.ConsecutiveBarsDown) Then
                Me.m_ConsDnSE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ConsUpLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "ConsUpLE", EOrderAction.Buy))
            Me.m_ConsDnSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "ConsDnSE", EOrderAction.SellShort))
            Me.m_ups_cnt = New VariableObject(Of Integer)(Me)
            Me.m_downs_cnt = New VariableObject(Of Integer)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property ConsecutiveBarsDown As Integer

        <Input()> _
        Public Property ConsecutiveBarsUp As Integer

        Private ReadOnly Property price As ISeries(Of Double)
            Get
                Return Me.m_price
            End Get
        End Property


        ' Fields
        Private m_ConsDnSE As IOrderMarket
        Private m_ConsUpLE As IOrderMarket
        Private m_downs_cnt As VariableObject(Of Integer)
        Private m_price As ISeries(Of Double)
        Private m_ups_cnt As VariableObject(Of Integer)
    End Class
End Namespace
