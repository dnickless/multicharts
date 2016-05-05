Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Percent_Trailing
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.TrailingPct = 20
            Me.FloorAmt = 1
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = If(Me.IsPositionBasis, ESpecOrdersMode.PerPosition, ESpecOrdersMode.PerContract)
            MyBase.GeneratePercentTrailing(Me.FloorAmt, Me.TrailingPct)
        End Sub


        ' Properties
        <Input> _
        Public Property FloorAmt As Double

        <Input()> _
        Public Property IsPositionBasis As Boolean

        <Input()> _
        Public Property TrailingPct As Double

    End Class
End Namespace
