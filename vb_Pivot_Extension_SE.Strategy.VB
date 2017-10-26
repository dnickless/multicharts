Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Pivot_Extension_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.RightStrength = 2
            Me.LeftStrength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_PivotHighVS.Item(0) > 0) Then
                Me.m_PivExtSE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_PivotHighVS = New [Function].PivotHighVS(Me)
            Me.m_PivExtSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "PivExtSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.HPrice = MyBase.Bars.High
            Me.m_PivotHighVS.instance = 1
            Me.m_PivotHighVS.price = Me.HPrice
            Me.m_PivotHighVS.leftstrength = Me.LeftStrength
            Me.m_PivotHighVS.rightstrength = Me.RightStrength
            Me.m_PivotHighVS.length = (Me.RightStrength + 1)
        End Sub


        ' Properties
        Private Property HPrice As ISeries(Of Double)

        <Input()> _
        Public Property LeftStrength As Integer

        <Input()> _
        Public Property RightStrength As Integer


        ' Fields
        Private m_PivExtSE As IOrderMarket
        Private m_PivotHighVS As [Function].PivotHighVS
    End Class
End Namespace
