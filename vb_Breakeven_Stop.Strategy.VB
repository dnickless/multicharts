Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Breakeven_Stop
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.FloorAmt = 1
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = If(Me.IsPositionBasis, ESpecOrdersMode.PerPosition, ESpecOrdersMode.PerContract)
            MyBase.GenerateBreakEven(Me.FloorAmt)
        End Sub


        ' Properties
        <Input> _
        Public Property FloorAmt As Double

        <Input()> _
        Public Property IsPositionBasis As Boolean

    End Class
End Namespace
