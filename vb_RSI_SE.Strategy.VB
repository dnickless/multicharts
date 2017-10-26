Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_RSI_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.OverBought = 70
            Me.Length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_myrsi.Value = Me.m_RSI.Item(0)
            If ((MyBase.Bars.CurrentBar > 1) AndAlso Me.CrossesUnder(Me.m_myrsi, Me.OverBought)) Then
                Me.m_RsiSE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_RSI = New RSI(Me)
            Me.m_myrsi = New VariableSeries(Of Double)(Me)
            Me.m_RsiSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "RsiSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
            Me.m_RSI.price = Me.Price
            Me.m_RSI.length = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property OverBought As Double

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_myrsi As VariableSeries(Of Double)
        Private m_RSI As RSI
        Private m_RsiSE As IOrderMarket
    End Class
End Namespace
