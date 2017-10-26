Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_TimeExit_Calndr_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumDays = 3
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_MP As Integer = MyBase.StrategyInfo.MarketPosition
            If (m_MP <> 0) Then
                Dim TargetTime As DateTime = (Me.EntryTime() + Me.m_Intervall)
                If ((TargetTime <= MyBase.Bars.Time.Item(0)) AndAlso (m_MP > 0)) Then
                    Me.m_TimeCalLX.Send
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_TimeCalLX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "TimeCalLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_Intervall = New TimeSpan(Me.NumDays, Me.NumHours, Me.NumMinutes, 0)
        End Sub


        ' Properties
        <Input> _
        Public Property NumDays As Integer

        <Input()> _
        Public Property NumHours As Integer

        <Input()> _
        Public Property NumMinutes As Integer


        ' Fields
        Private m_Intervall As TimeSpan
        Private m_TimeCalLX As IOrderMarket
    End Class
End Namespace
