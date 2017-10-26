Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System

Namespace PowerLanguage.Strategy
    Public Class vb_Gap_Down_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleLess(MyBase.Bars.High.Item(0), MyBase.Bars.Low.Item(1)) Then
                Me.m_GapDn.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_GapDn = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "GapDn", EOrderAction.SellShort))
        End Sub


        ' Fields
        Private m_GapDn As IOrderMarket
    End Class
End Namespace
