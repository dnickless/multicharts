Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Custom_Strategy_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.SE_Condition.Item(0) Then
                Me.m_CustomSE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_CustomSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "CustomSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.SE_Condition = New Lambda(Of Boolean)(Function(_bb) PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(_bb), MyBase.Bars.Open.Item(_bb)))
        End Sub


        ' Properties
        Private Property SE_Condition As ISeries(Of Boolean)

        ' Fields
        Private m_CustomSE As IOrderMarket
    End Class
End Namespace
