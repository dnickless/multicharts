Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Inside_Bar_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If ((PublicFunctions.DoubleLess(MyBase.Bars.High.Item(0), MyBase.Bars.High.Item(1)) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), MyBase.Bars.Low.Item(1))) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(0), MyBase.Bars.Open.Item(0))) Then
                Me.m_InsBarLE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_InsBarLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "InsBarLE", EOrderAction.Buy))
        End Sub


        ' Fields
        Private m_InsBarLE As IOrderMarket
    End Class
End Namespace
