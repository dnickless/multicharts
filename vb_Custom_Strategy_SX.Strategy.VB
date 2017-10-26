Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Custom_Strategy_SX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.SX_Condition.Item(0) Then
                Me.m_CustomSX.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_CustomSX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "CustomSX", EOrderAction.BuyToCover, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.SX_Condition = New Lambda(Of Boolean)(Function(_bb) PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(_bb), MyBase.Bars.Open.Item(_bb)))
        End Sub


        ' Properties
        Private Property SX_Condition As ISeries(Of Boolean)


        ' Fields
        Private m_CustomSX As IOrderMarket
    End Class
End Namespace
