Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Channel_Breakout_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ChBrkSE.Send((Me.m_LowestFC.Item(0) - MyBase.Bars.Point))
        End Sub

        Protected Overrides Sub Create()
            Me.m_LowestFC = New LowestFC(Me)
            Me.m_ChBrkSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ChBrkSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Low
            Me.m_LowestFC.pricevalue = Me.Price
            Me.m_LowestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_ChBrkSE As IOrderPriced
        Private m_LowestFC As LowestFC
    End Class
End Namespace
