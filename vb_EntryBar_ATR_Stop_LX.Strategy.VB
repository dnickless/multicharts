Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_EntryBar_ATR_Stop_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumAtrs = 1
            Me.AtrLength = 5
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.StrategyInfo.MarketPosition <> 1) Then
                Me.m_EbAtrLX.Send((MyBase.Bars.Close.Item(0) - (Me.AverageTrueRange(Me.AtrLength) * Me.NumAtrs)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_EbAtrLX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "EbAtrLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property AtrLength As Integer

        <Input()> _
        Public Property NumAtrs As Integer


        ' Fields
        Private m_EbAtrLX As IOrderPriced
    End Class
End Namespace
