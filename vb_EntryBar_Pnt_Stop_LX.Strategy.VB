Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_EntryBar_Pnt_Stop_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.RiskPoints = 10
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.StrategyInfo.MarketPosition <> 1) Then
                Me.m_EbPntLX.Send((MyBase.Bars.Close.Item(0) - (Me.RiskPoints * MyBase.Bars.Point)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_EbPntLX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "EbPntLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property RiskPoints As Double


        ' Fields
        Private m_EbPntLX As IOrderPriced
    End Class
End Namespace
