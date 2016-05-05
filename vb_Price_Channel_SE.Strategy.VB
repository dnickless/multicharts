Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Price_Channel_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_PChSE.Send((Me.m_LowestFC.Item(0) - MyBase.Bars.Point))
        End Sub

        Protected Overrides Sub Create()
            Me.m_LowestFC = New LowestFC(Me)
            Me.m_PChSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "PChSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_LowestFC.pricevalue = MyBase.Bars.Low
            Me.m_LowestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer


        ' Fields
        Private m_LowestFC As LowestFC
        Private m_PChSE As IOrderPriced
    End Class
End Namespace
