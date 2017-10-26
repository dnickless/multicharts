Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MACD_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.FastLength = 12
            Me.SlowLength = &H1A
            Me.MacdLength = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_my_MACD.Value = Me.m_MACD.Item(0)
            Dim MACD_avg As Double = Me.m_XAverage.Item(0)
            Me.m_MACD_diff.Value = (Me.m_my_MACD.Value - MACD_avg)
            If ((MyBase.Bars.CurrentBar > 2) AndAlso PublicFunctions.CrossesUnder(DirectCast(Me.m_MACD_diff, ISeries(Of Double)), CDbl(0), MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_MacdSE.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_MACD = New MACD(Me)
            Me.m_XAverage = New XAverage(Me)
            Me.m_my_MACD = New VariableSeries(Of Double)(Me)
            Me.m_MACD_diff = New VariableSeries(Of Double)(Me)
            Me.m_MacdSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MacdSE", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_MACD.Price = MyBase.Bars.Close
            Me.m_MACD.FastLength = Me.FastLength
            Me.m_MACD.SlowLength = Me.SlowLength
            Me.m_XAverage.Price = Me.m_my_MACD
            Me.m_XAverage.Length = Me.MacdLength
        End Sub


        ' Properties
        <Input> _
        Public Property FastLength As Integer

        <Input()> _
        Public Property MacdLength As Integer

        <Input()> _
        Public Property SlowLength As Integer


        ' Fields
        Private m_MACD As MACD
        Private m_MACD_diff As VariableSeries(Of Double)
        Private m_MacdSE As IOrderMarket
        Private m_my_MACD As VariableSeries(Of Double)
        Private m_XAverage As XAverage
    End Class
End Namespace
