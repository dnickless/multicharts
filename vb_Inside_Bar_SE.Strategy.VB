Imports PowerLanguage
Imports System

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Inside_Bar_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If ((PublicFunctions.DoubleLess(MyBase.Bars.High.Item(0), MyBase.Bars.High.Item(1)) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), MyBase.Bars.Low.Item(1))) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), MyBase.Bars.Open.Item(0))) Then
                Me.m_InsBarSE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_InsBarSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "InsBarSE", EOrderAction.SellShort))
        End Sub


        ' Fields
        Private m_InsBarSE As IOrderMarket
    End Class
End Namespace
