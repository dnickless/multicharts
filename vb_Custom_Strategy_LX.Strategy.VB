Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Custom_Strategy_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.LX_Condition.Item(0) Then
                Me.m_CustomLX.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_CustomLX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "CustomLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.LX_Condition = New Lambda(Of Boolean)(Function(_bb) PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(_bb), MyBase.Bars.Open.Item(_bb)))
        End Sub


        ' Properties
        Private Property LX_Condition As ISeries(Of Boolean)


        ' Fields
        Private m_CustomLX As IOrderMarket
    End Class
End Namespace
