Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Stochastic_Slow_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 14
            Me.OverSold = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_Stochastic.Call
            If (((MyBase.Bars.CurrentBar > 2) AndAlso PublicFunctions.CrossesOver(DirectCast(Me.m_oSlowK, ISeries(Of Double)), DirectCast(Me.m_oSlowD, ISeries(Of Double)), MyBase.ExecInfo.MaxBarsBack)) AndAlso PublicFunctions.DoubleLess(Me.m_oSlowK.Value, Me.OverSold)) Then
                Me.m_StochLE.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_Stochastic = New Stochastic(Me)
            Me.m_oFastK = New VariableSeries(Of Double)(Me)
            Me.m_oFastD = New VariableSeries(Of Double)(Me)
            Me.m_oSlowK = New VariableSeries(Of Double)(Me)
            Me.m_oSlowD = New VariableSeries(Of Double)(Me)
            Me.m_StochLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "StochLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_Stochastic.priceh = MyBase.Bars.High
            Me.m_Stochastic.pricel = MyBase.Bars.Low
            Me.m_Stochastic.pricec = MyBase.Bars.Close
            Me.m_Stochastic.stochlength = Me.Length
            Me.m_Stochastic.length1 = 3
            Me.m_Stochastic.length2 = 3
            Me.m_Stochastic.smoothingtype = 1
            Me.m_Stochastic.ofastk = Me.m_oFastK
            Me.m_Stochastic.ofastd = Me.m_oFastD
            Me.m_Stochastic.oslowk = Me.m_oSlowK
            Me.m_Stochastic.oslowd = Me.m_oSlowD
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property OverSold As Double


        ' Fields
        Private m_oFastD As VariableSeries(Of Double)
        Private m_oFastK As VariableSeries(Of Double)
        Private m_oSlowD As VariableSeries(Of Double)
        Private m_oSlowK As VariableSeries(Of Double)
        Private m_Stochastic As Stochastic
        Private m_StochLE As IOrderMarket
    End Class
End Namespace
