Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Volty_Expan_Close_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumAtrs = 1.5
            Me.Length = 5
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_VltClsSX.Send((MyBase.Bars.Close.Item(0) + (Me.AverageTrueRange(Me.Length) * Me.NumAtrs)))
        End Sub

        Protected Overrides Sub Create()
            Me.m_VltClsSX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "VltClsSX", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property NumAtrs As Double


        ' Fields
        Private m_VltClsSX As IOrderPriced
    End Class
End Namespace
