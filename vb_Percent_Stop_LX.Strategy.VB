Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Percent_Stop_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.StopLossPct = 0.05
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = ESpecOrdersMode.PerContract
            If (MyBase.StrategyInfo.MarketPosition > 0) Then
                MyBase.GenerateStopLoss((Me.EntryPrice() * Me.StopLossPct))
            Else
                Me.m_PctStopLX_eb.Send((MyBase.Bars.Close.Item(0) * (1 - Me.StopLossPct)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_PctStopLX_eb = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "PctStopLX-eb", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property StopLossPct As Double


        ' Fields
        Private m_PctStopLX_eb As IOrderPriced
    End Class
End Namespace
