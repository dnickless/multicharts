Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Channel_Trailing_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 3
            Me.FloorAmt = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Dim FloorProfit As Double = If(Me.IsPositionBasis, Me.FloorAmt, (-MyBase.StrategyInfo.MarketPosition * Me.FloorAmt))
            Dim CoverPrice As Double = Me.m_HighestFC.Item(0)
            If ((MyBase.StrategyInfo.MarketPosition < 0) AndAlso PublicFunctions.DoubleGreaterEquals(Me.MaxPositionProfit(), FloorProfit)) Then
                Me.m_ChTrSX.Send(CoverPrice)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_HighestFC = New HighestFC(Me)
            Me.m_ChTrSX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ChTrSX", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_HighestFC.pricevalue = MyBase.Bars.High
            Me.m_HighestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property FloorAmt As Double

        <Input()> _
        Public Property IsPositionBasis As Boolean

        <Input()> _
        Public Property Length As Integer


        ' Fields
        Private m_ChTrSX As IOrderPriced
        Private m_HighestFC As HighestFC
    End Class
End Namespace
