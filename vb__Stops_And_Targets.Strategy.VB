Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb__Stops_And_Targets
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.StopLossAmt = 1
            Me.ProfitTargetAmt = 5
            Me.IsPerContractOption = True
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = If(Me.IsPerContractOption, ESpecOrdersMode.PerContract, ESpecOrdersMode.PerPosition)
            If PublicFunctions.DoubleGreater(Me.ProfitTargetAmt, 0) Then
                MyBase.GenerateProfitTarget(Me.ProfitTargetAmt)
            End If
            If PublicFunctions.DoubleGreater(Me.StopLossAmt, 0) Then
                MyBase.GenerateStopLoss(Me.StopLossAmt)
            End If
            If PublicFunctions.DoubleGreater(Me.BreakEvenFloorAmt, 0) Then
                MyBase.GenerateBreakEven(Me.BreakEvenFloorAmt)
            End If
            If PublicFunctions.DoubleGreater(Me.DollarTrailingAmt, 0) Then
                MyBase.GenerateDollarTrailing(Me.DollarTrailingAmt)
            End If
            If (PublicFunctions.DoubleGreater(Me.PctTrailingFloorAmt, 0) AndAlso PublicFunctions.DoubleGreater(Me.PctTrailingPct, 0)) Then
                MyBase.GeneratePercentTrailing(Me.PctTrailingFloorAmt, Me.PctTrailingPct)
            End If
            If Me.IsSetExitOnClose Then
                MyBase.GenerateExitOnClose
            End If
        End Sub


        ' Properties
        <Input> _
        Public Property BreakEvenFloorAmt As Double

        <Input()> _
        Public Property DollarTrailingAmt As Double

        <Input()> _
        Public Property IsPerContractOption As Boolean

        <Input()> _
        Public Property IsSetExitOnClose As Boolean

        <Input()> _
        Public Property PctTrailingFloorAmt As Double

        <Input()> _
        Public Property PctTrailingPct As Double

        <Input()> _
        Public Property ProfitTargetAmt As Double

        <Input()> _
        Public Property StopLossAmt As Double

    End Class
End Namespace
