Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MovAvg2Line_Cross_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.FastLength = 9
            Me.SlowLength = &H12
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_FastAvg.Value = Me.m_FastAverageFC.Item(0)
            Me.m_SlowAvg.Value = Me.m_SlowAverageFC.Item(0)
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesUnder(DirectCast(Me.m_FastAvg, ISeries(Of Double)), DirectCast(Me.m_SlowAvg, ISeries(Of Double)), MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_MA2CrossSE.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_FastAverageFC = New [Function].AverageFC(Me)
            Me.m_SlowAverageFC = New [Function].AverageFC(Me)
            Me.m_FastAvg = New VariableSeries(Of Double)(Me)
            Me.m_SlowAvg = New VariableSeries(Of Double)(Me)
            Me.m_MA2CrossSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MA2CrossSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
            Me.m_FastAverageFC.price = Me.Price
            Me.m_FastAverageFC.length = Me.FastLength
            Me.m_SlowAverageFC.price = Me.Price
            Me.m_SlowAverageFC.length = Me.SlowLength
        End Sub


        ' Properties
        <Input> _
        Public Property FastLength As Integer

        Private Property Price As ISeries(Of Double)

        <Input()> _
        Public Property SlowLength As Integer


        ' Fields
        Private m_FastAverageFC As [Function].AverageFC
        Private m_FastAvg As VariableSeries(Of Double)
        Private m_MA2CrossSE As IOrderMarket
        Private m_SlowAverageFC As [Function].AverageFC
        Private m_SlowAvg As VariableSeries(Of Double)
    End Class
End Namespace
