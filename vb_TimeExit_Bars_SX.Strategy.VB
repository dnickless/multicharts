Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_TimeExit_Bars_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.BarToExitOn = 5
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.BarsSinceEntry() = Me.BarToExitOn) Then
                Me.m_TimeBarsSX.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_TimeBarsSX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "TimeBarsSX", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property BarToExitOn As Integer


        ' Fields
        Private m_TimeBarsSX As IOrderMarket
    End Class
End Namespace
