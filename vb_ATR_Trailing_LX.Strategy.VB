Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_ATR_Trailing_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumAtrs = 3
            Me.AtrLength = 10
        End Sub

        Protected Overrides Sub CalcBar()
            Dim AtrValue As Double = (Me.AverageTrueRange(Me.AtrLength) * Me.NumAtrs)
            Me.m_mp.Value = MyBase.StrategyInfo.MarketPosition
            If (Me.m_mp.Value > 0) Then
                If ((Me.m_mp.Item(1) <= 0) OrElse PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_PosHigh.Value)) Then
                    Me.m_PosHigh.Value = MyBase.Bars.High.Item(0)
                End If
                Me.m_AtrLX.Send((Me.m_PosHigh.Value - AtrValue))
            Else
                Me.m_AtrLX_eb.Send((MyBase.Bars.High.Item(0) - AtrValue))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mp = New VariableSeries(Of Integer)(Me)
            Me.m_PosHigh = New VariableObject(Of Double)(Me)
            Me.m_AtrLX = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "AtrLX", EOrderAction.Sell, OrderExit.FromAll))
            Me.m_AtrLX_eb = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "AtrLX-eb", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property AtrLength As Integer

        <Input()> _
        Public Property NumAtrs As Integer


        ' Fields
        Private m_AtrLX As IOrderPriced
        Private m_AtrLX_eb As IOrderPriced
        Private m_mp As VariableSeries(Of Integer)
        Private m_PosHigh As VariableObject(Of Double)
    End Class
End Namespace
