Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MovAvg3Line_Cross_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.MedLength = 9
            Me.SlowLength = &H12
            Me.FastLength = 4
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_FastAvg As Double = Me.m_FastAverageFC.Item(0)
            Dim m_MedAvg As Double = Me.m_MedAverageFC.Item(0)
            Dim m_SlowAvg As Double = Me.m_SlowAverageFC.Item(0)
            Me.m_Cond.Value = ((PublicFunctions.DoubleGreater(Me.Price.Item(0), m_FastAvg) AndAlso PublicFunctions.DoubleGreater(m_FastAvg, m_MedAvg)) AndAlso PublicFunctions.DoubleGreater(m_MedAvg, m_SlowAvg))
            If Not If(((MyBase.Bars.CurrentBar <= 1) OrElse Not Me.m_Cond.Value), True, Me.m_Cond.Item(1)) Then
                Me.m_MA3CrsLE.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_FastAverageFC = New AverageFC(Me)
            Me.m_MedAverageFC = New AverageFC(Me)
            Me.m_SlowAverageFC = New AverageFC(Me)
            Me.m_Cond = New VariableSeries(Of Boolean)(Me)
            Me.m_MA3CrsLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MA3CrsLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
            Me.m_FastAverageFC.price = Me.Price
            Me.m_FastAverageFC.length = Me.FastLength
            Me.m_MedAverageFC.price = Me.Price
            Me.m_MedAverageFC.length = Me.MedLength
            Me.m_SlowAverageFC.price = Me.Price
            Me.m_SlowAverageFC.length = Me.SlowLength
        End Sub


        ' Properties
        <Input> _
        Public Property FastLength As Integer

        <Input()> _
        Public Property MedLength As Integer

        Private Property Price As ISeries(Of Double)

        <Input()> _
        Public Property SlowLength As Integer


        ' Fields
        Private m_Cond As VariableSeries(Of Boolean)
        Private m_FastAverageFC As AverageFC
        Private m_MA3CrsLE As IOrderMarket
        Private m_MedAverageFC As AverageFC
        Private m_SlowAverageFC As AverageFC
    End Class
End Namespace
