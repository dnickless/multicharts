Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Channel_Breakout_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_ChBrkLE.Send((Me.m_HighestFC.Item(0) + MyBase.Bars.Point))
        End Sub

        Protected Overrides Sub Create()
            Me.m_HighestFC = New HighestFC(Me)
            Me.m_ChBrkLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "ChBrkLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.High
            Me.m_HighestFC.pricevalue = Me.Price
            Me.m_HighestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_ChBrkLE As IOrderPriced
        Private m_HighestFC As HighestFC
    End Class
End Namespace
