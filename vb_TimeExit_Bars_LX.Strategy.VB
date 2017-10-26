Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_TimeExit_Bars_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.BarToExitOn = 5
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.BarsSinceEntry() = Me.BarToExitOn) Then
                Me.m_TimeBarsLX.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_TimeBarsLX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "TimeBarsLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property BarToExitOn As Integer


        ' Fields
        Private m_TimeBarsLX As IOrderMarket
    End Class
End Namespace
