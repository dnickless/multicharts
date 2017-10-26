Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Pivot_Reversal_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Strength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_SwingLow.Item(0) > 0) Then
                Me.m_Setup.Value = True
                Me.m_PivotLow.Value = MyBase.Bars.Low.Item(Me.Strength)
            ElseIf (Me.m_Setup.Value AndAlso PublicFunctions.DoubleLessEquals(MyBase.Bars.Low.Item(0), (Me.m_PivotLow.Value - MyBase.Bars.Point))) Then
                Me.m_Setup.Value = False
            End If
            If Me.m_Setup.Value Then
                Me.m_PivRevSE.Send((Me.m_PivotLow.Value - MyBase.Bars.Point))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_SwingLow = New SwingLow(Me)
            Me.m_Setup = New VariableObject(Of Boolean)(Me)
            Me.m_PivotLow = New VariableObject(Of Double)(Me)
            Me.m_PivRevSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "PivRevSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_SwingLow.instance = 1
            Me.m_SwingLow.price = MyBase.Bars.Low
            Me.m_SwingLow.strength = Me.Strength
            Me.m_SwingLow.length = (Me.Strength + 1)
        End Sub


        ' Properties
        <Input> _
        Public Property Strength As Integer


        ' Fields
        Private m_PivotLow As VariableObject(Of Double)
        Private m_PivRevSE As IOrderPriced
        Private m_Setup As VariableObject(Of Boolean)
        Private m_SwingLow As SwingLow
    End Class
End Namespace
