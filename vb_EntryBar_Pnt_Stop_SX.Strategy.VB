Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_EntryBar_Pnt_Stop_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.RiskPoints = 10
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.StrategyInfo.MarketPosition <> -1) Then
                Me.m_EbPntSX.Send((MyBase.Bars.Close.Item(0) + (Me.RiskPoints * MyBase.Bars.Point)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_EbPntSX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "EbPntSX", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property RiskPoints As Double


        ' Fields
        Private m_EbPntSX As IOrderPriced
    End Class
End Namespace
