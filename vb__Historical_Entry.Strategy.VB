Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb__Historical_Entry
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.PrevBarTime = DateTime.Now
            Me.IsLongOrder = True
            Me.MyEntryPrice = 100
            Me.Quantity = &H3E8
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.m_InitialPass Then
            End If
            If (PublicFunctions.IsDayBased(MyBase.Bars.Info.Resolution.Type) AndAlso ((MyBase.Bars.Time.Item(0).Date = Me.PrevBarTime.Date) OrElse (MyBase.Bars.Time.Item(0) = Me.PrevBarTime))) Then
                If Me.IsLongOrder Then
                    If PublicFunctions.DoubleGreater(Me.MyEntryPrice, CDbl(Me.OpenPriceEntryBar)) Then
                        Me.m_StopBuy.Send(Me.MyEntryPrice, Me.Quantity)
                    Else
                        Me.m_LimitBuy.Send(Me.MyEntryPrice, Me.Quantity)
                    End If
                ElseIf PublicFunctions.DoubleLess(Me.MyEntryPrice, CDbl(Me.OpenPriceEntryBar)) Then
                    Me.m_StopShort.Send(Me.MyEntryPrice, Me.Quantity)
                Else
                    Me.m_LimitShort.Send(Me.MyEntryPrice, Me.Quantity)
                End If
            End If
            If (Me.m_InitialPass AndAlso MyBase.Bars.LastBarOnChart) Then
                Me.m_InitialPass = False
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_StopBuy = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Buy", EOrderAction.Buy))
            Me.m_LimitBuy = MyBase.OrderCreator.Limit(New SOrderParameters(Contracts.Default, "Buy#1", EOrderAction.Buy))
            Me.m_StopShort = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Short", EOrderAction.SellShort))
            Me.m_LimitShort = MyBase.OrderCreator.Limit(New SOrderParameters(Contracts.Default, "Short#1", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_InitialPass = True
        End Sub


        ' Properties
        <Input> _
        Public Property IsLongOrder As Boolean

        <Input()> _
        Public Property MyEntryPrice As Double

        <Input()> _
        Public Property OpenPriceEntryBar As Integer

        <Input()> _
        Public Property PrevBarTime As DateTime

        <Input()> _
        Public Property Quantity As Integer


        ' Fields
        Private m_InitialPass As Boolean
        Private m_LimitBuy As IOrderPriced
        Private m_LimitShort As IOrderPriced
        Private m_StopBuy As IOrderPriced
        Private m_StopShort As IOrderPriced
    End Class
End Namespace
