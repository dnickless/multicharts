Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System

Namespace PowerLanguage.Strategy
    Public Class vb_Gap_Up_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), MyBase.Bars.High.Item(1)) Then
                Me.m_GapUp.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_GapUp = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "GapUp", EOrderAction.Buy))
        End Sub


        ' Fields
        Private m_GapUp As IOrderMarket
    End Class
End Namespace
