Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_EntryBar__Stop_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.DollarRisk = 1
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.StrategyInfo.MarketPosition <> 1) Then
                Me.m_EbDlrLX.Send((MyBase.Bars.Close.Item(0) - Me.m_BigPointRisk))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_EbDlrLX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "EbDlrLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_BigPointRisk = (Me.DollarRisk / MyBase.Bars.Info.BigPointValue)
        End Sub


        ' Properties
        <Input> _
        Public Property DollarRisk As Double


        ' Fields
        Private m_BigPointRisk As Double
        Private m_EbDlrLX As IOrderPriced
    End Class
End Namespace
