Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_ATR_Trailing_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumAtrs = 3
            Me.AtrLength = 10
        End Sub

        Protected Overrides Sub CalcBar()
            Dim AtrVal As Double = (Me.AverageTrueRange(Me.AtrLength) * Me.NumAtrs)
            Me.m_mp.Value = MyBase.StrategyInfo.MarketPosition
            If (Me.m_mp.Value < 0) Then
                If ((Me.m_mp.Item(1) >= 0) OrElse PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_PosLow.Value)) Then
                    Me.m_PosLow.Value = MyBase.Bars.Low.Item(0)
                End If
                Me.m_AtrSX.Send((Me.m_PosLow.Value + AtrVal))
            Else
                Me.m_AtrSX_eb.Send((MyBase.Bars.Low.Item(0) + AtrVal))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mp = New VariableSeries(Of Integer)(Me)
            Me.m_PosLow = New VariableObject(Of Double)(Me)
            Me.m_AtrSX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "AtrSX", EOrderAction.BuyToCover, OrderExit.FromAll))
            Me.m_AtrSX_eb = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "AtrSX-eb", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property AtrLength As Integer

        <Input()> _
        Public Property NumAtrs As Integer


        ' Fields
        Private m_AtrSX As IOrderPriced
        Private m_AtrSX_eb As IOrderPriced
        Private m_mp As VariableSeries(Of Integer)
        Private m_PosLow As VariableObject(Of Double)
    End Class
End Namespace
