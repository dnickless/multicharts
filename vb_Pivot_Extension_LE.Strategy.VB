Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Pivot_Extension_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.RightStrength = 2
            Me.LeftStrength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_PivotLowVS.Item(0) > 0) Then
                Me.m_PivExtLE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_PivotLowVS = New PivotLowVS(Me)
            Me.m_PivExtLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "PivExtLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.LPrice = MyBase.Bars.Low
            Me.m_PivotLowVS.instance = 1
            Me.m_PivotLowVS.price = Me.LPrice
            Me.m_PivotLowVS.leftstrength = Me.LeftStrength
            Me.m_PivotLowVS.rightstrength = Me.RightStrength
            Me.m_PivotLowVS.length = (Me.RightStrength + 1)
        End Sub


        ' Properties
        <Input> _
        Public Property LeftStrength As Integer

        Private Property LPrice As ISeries(Of Double)

        <Input()> _
        Public Property RightStrength As Integer


        ' Fields
        Private m_PivExtLE As IOrderMarket
        Private m_PivotLowVS As PivotLowVS
    End Class
End Namespace
