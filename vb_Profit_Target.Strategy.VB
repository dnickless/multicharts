Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Profit_Target
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Amount = 5
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = If(Me.IsPositionBasis, ESpecOrdersMode.PerPosition, ESpecOrdersMode.PerContract)
            MyBase.GenerateProfitTarget(Me.Amount)
        End Sub


        ' Properties
        <Input> _
        Public Property Amount As Double

        <Input()> _
        Public Property IsPositionBasis As Boolean

    End Class
End Namespace
