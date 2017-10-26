Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Price_Channel_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_PChLE.Send((Me.m_HighestFC.Item(0) + MyBase.Bars.Point))
        End Sub

        Protected Overrides Sub Create()
            Me.m_HighestFC = New HighestFC(Me)
            Me.m_PChLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "PChLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_HighestFC.pricevalue = MyBase.Bars.High
            Me.m_HighestFC.len = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer


        ' Fields
        Private m_HighestFC As HighestFC
        Private m_PChLE As IOrderPriced
    End Class
End Namespace
