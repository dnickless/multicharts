Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Key_Reversal_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 1
        End Sub

        Protected Overrides Sub CalcBar()
            m_highest.Value = Bars.High.Highest(Length)
            If (PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), m_highest.Item(1)) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), MyBase.Bars.Close.Item(1))) Then
                Me.m_KeyRevLX.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            m_highest = New VariableSeries(Of Double)(Me)
            Me.m_KeyRevLX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "KeyRevLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer


        ' Fields
        Private m_highest As VariableSeries(Of Double)
        Private m_KeyRevLX As IOrderMarket
    End Class
End Namespace
