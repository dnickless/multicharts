Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_MomentumSE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 12
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_mom.Value = Me.Price.Momentum(Me.Length)
            Dim m_accel As Double = Me.m_mom.Momentum(1)
            If (PublicFunctions.DoubleLess(Me.m_mom.Value, 0) AndAlso PublicFunctions.DoubleLess(m_accel, 0)) Then
                Me.m_MomSE.Send((MyBase.Bars.Low.Item(0) - MyBase.Bars.Point))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_mom = New VariableSeries(Of Double)(Me)
            Me.m_MomSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "MomSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_mom As VariableSeries(Of Double)
        Private m_MomSE As IOrderPriced
    End Class
End Namespace
