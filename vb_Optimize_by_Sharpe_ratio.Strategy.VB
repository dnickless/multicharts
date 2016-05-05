Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Optimize_by_Sharpe_ratio
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Bars.LastBarOnChart Then
                MyBase.CustomFitnessValue = Me.m_SharpeRatio.Value
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_SharpeRatio = New SharpeRatio(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_SharpeRatio.Period = Me.Period
        End Sub


        ' Properties
        <Input()> _
        Public Property Period As SharpeRatio.EPeriod


        ' Fields
        Private m_SharpeRatio As SharpeRatio
    End Class
End Namespace
