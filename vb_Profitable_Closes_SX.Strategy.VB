Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Profitable_Closes_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumProfitCloses = 5
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_MP.Value = 0
            If (MyBase.StrategyInfo.MarketPosition > 0) Then
                Me.m_MP.Value = 1
            End If
            If (MyBase.StrategyInfo.MarketPosition < 0) Then
                Me.m_MP.Value = -1
            End If
            If (Not Me.m_MP.Value = 0) Then
                If (Not Me.m_MP.Item(1) = Me.m_MP.Value) Then
                    Me.m_cnt.Value = 0
                    Me.m_EntryPrice.Value = Me.EntryPrice()
                End If
                If (PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), Me.m_EntryPrice.Value) AndAlso PublicFunctions.DoubleLess(CDbl(Me.m_MP.Value), 0)) Then
                    Me.m_cnt.Value = (Me.m_cnt.Value + 1)
                End If
                If (Me.m_cnt.Value = Me.NumProfitCloses) Then
                    Me.m_PftClsSX.Send()
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_MP = New VariableSeries(Of Integer)(Me)
            Me.m_PftClsSX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "PftClsSX", EOrderAction.BuyToCover, OrderExit.FromAll))
            Me.m_EntryPrice = New VariableObject(Of Double)(Me)
            Me.m_cnt = New VariableObject(Of Integer)(Me)
        End Sub


        ' Properties
        <Input> _
        Public Property NumProfitCloses As Integer


        ' Fields
        Private m_cnt As VariableObject(Of Integer)
        Private m_EntryPrice As VariableObject(Of Double)
        Private m_MP As VariableSeries(Of Integer)
        Private m_PftClsSX As IOrderMarket
    End Class
End Namespace
