Imports PowerLanguage
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb__Percent_Exit_Pair
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.StopLossPct = 0.05
            Me.ProfitTargetPct = 0.1
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.CurSpecOrdersMode = ESpecOrdersMode.PerContract
            If (MyBase.StrategyInfo.MarketPosition <> 0) Then
                If PublicFunctions.DoubleGreater(Me.ProfitTargetPct, 0) Then
                    MyBase.GenerateProfitTarget((Me.EntryPrice() * Me.ProfitTargetPct))
                End If
                If PublicFunctions.DoubleGreater(Me.StopLossPct, 0) Then
                    MyBase.GenerateStopLoss((Me.EntryPrice() * Me.StopLossPct))
                End If
            Else
                If PublicFunctions.DoubleGreater(Me.ProfitTargetPct, 0) Then
                    MyBase.GenerateProfitTarget((MyBase.Bars.Close.Item(0) * Me.ProfitTargetPct))
                End If
                If PublicFunctions.DoubleGreater(Me.StopLossPct, 0) Then
                    MyBase.GenerateStopLoss((MyBase.Bars.Close.Item(0) * Me.StopLossPct))
                End If
            End If
        End Sub


        ' Properties
        <Input> _
        Public Property ProfitTargetPct As Double

        <Input()> _
        Public Property StopLossPct As Double

    End Class
End Namespace
