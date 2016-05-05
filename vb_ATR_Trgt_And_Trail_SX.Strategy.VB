Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_ATR_Trgt_And_Trail_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumBars = 5
            Me.NumAtrs = 2
            Me.AtrLength = 5
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mp.Value = MyBase.StrategyInfo.MarketPosition
            If (Me.m_mp.Value < 0) Then
                If (Me.m_mp.Item(1) >= 0) Then
                    Me.m_TargetPrice.Value = (Me.EntryPrice() - (Me.AverageTrueRange(Me.AtrLength) * Me.NumAtrs))
                End If
                If (Me.BarsSinceEntry() < Me.NumBars) Then
                    Me.m_ATTSX_Tgt.Send(Me.m_TargetPrice.Value)
                Else
                    Me.m_ATTSX_Trl.Send(MyBase.Bars.High.Item(0))
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mp = New VariableSeries(Of Integer)(Me)
            Me.m_ATTSX_Tgt = MyBase.OrderCreator.Limit(New SOrderParameters(Contracts.Default, "ATTSX-Tgt", EOrderAction.BuyToCover, OrderExit.FromAll))
            Me.m_ATTSX_Trl = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ATTSX-Trl", EOrderAction.BuyToCover, OrderExit.FromAll))
            Me.m_TargetPrice = New VariableObject(Of Double)(Me)
        End Sub


        ' Properties
        <Input> _
        Public Property AtrLength As Integer

        <Input()> _
        Public Property NumAtrs As Integer

        <Input()> _
        Public Property NumBars As Integer


        ' Fields
        Private m_ATTSX_Tgt As IOrderPriced
        Private m_ATTSX_Trl As IOrderPriced
        Private m_mp As VariableSeries(Of Integer)
        Private m_TargetPrice As VariableObject(Of Double)
    End Class
End Namespace
