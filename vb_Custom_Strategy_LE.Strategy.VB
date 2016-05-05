Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Custom_Strategy_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.LE_Condition.Item(0) Then
                Me.m_CustomLE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_CustomLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "CustomLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.LE_Condition = New Lambda(Of Boolean)(Function(_bb) PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(_bb), MyBase.Bars.Open.Item(_bb)))
        End Sub


        ' Properties
        Private Property LE_Condition As ISeries(Of Boolean)


        ' Fields
        Private m_CustomLE As IOrderMarket
    End Class
End Namespace
