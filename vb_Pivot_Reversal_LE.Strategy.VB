Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Pivot_Reversal_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Strength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.m_SwingHigh.Item(0) > 0) Then
                Me.m_Setup.Value = True
                Me.m_PivotHigh.Value = MyBase.Bars.High.Item(Me.Strength)
            ElseIf (Me.m_Setup.Value AndAlso PublicFunctions.DoubleGreaterEquals(MyBase.Bars.High.Item(0), (Me.m_PivotHigh.Value + MyBase.Bars.Point))) Then
                Me.m_Setup.Value = False
            End If
            If Me.m_Setup.Value Then
                Me.m_PivRevLE.Send((Me.m_PivotHigh.Value + MyBase.Bars.Point))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_SwingHigh = New SwingHigh(Me)
            Me.m_PivRevLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "PivRevLE", EOrderAction.Buy))
            Me.m_Setup = New VariableObject(Of Boolean)(Me)
            Me.m_PivotHigh = New VariableObject(Of Double)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_SwingHigh.instance = 1
            Me.m_SwingHigh.price = MyBase.Bars.High
            Me.m_SwingHigh.strength = Me.Strength
            Me.m_SwingHigh.length = (Me.Strength + 1)
        End Sub


        ' Properties
        <Input> _
        Public Property Strength As Integer


        ' Fields
        Private m_PivotHigh As VariableObject(Of Double)
        Private m_PivRevLE As IOrderPriced
        Private m_Setup As VariableObject(Of Boolean)
        Private m_SwingHigh As SwingHigh
    End Class
End Namespace
