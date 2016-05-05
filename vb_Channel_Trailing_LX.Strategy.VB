Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Channel_Trailing_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.FloorAmt = 1
            Me.Length = 3
        End Sub

        Protected Overrides Sub CalcBar()
            Dim FloorProfit As Double = If(Me.IsPositionBasis, Me.FloorAmt, (Me.FloorAmt * MyBase.StrategyInfo.MarketPosition))
            Dim SellPrice As Double = Me.m_LowestFC.Item(0)
            If ((MyBase.StrategyInfo.MarketPosition > 0) AndAlso PublicFunctions.DoubleGreaterEquals(Me.MaxPositionProfit(), FloorProfit)) Then
                Me.m_ChTrLX.Send(SellPrice)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_LowestFC = New LowestFC(Me)
            Me.m_ChTrLX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ChTrLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_LowestFC.pricevalue = MyBase.Bars.Low
            Me.m_LowestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property FloorAmt As Double

        <Input()> _
        Public Property IsPositionBasis As Boolean

        <Input()> _
        Public Property Length As Integer


        ' Fields
        Private m_ChTrLX As IOrderPriced
        Private m_LowestFC As LowestFC
    End Class
End Namespace
