Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MovAvg_Cross_LX
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_Avg.Value = Me.m_AverageFC.Item(0)
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesUnder(MyBase.Bars.Close, Me.m_Avg, MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_MACrossLX.Send
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New AverageFC(Me)
            Me.m_Avg = New VariableSeries(Of Double)(Me)
            Me.m_MACrossLX = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MACrossLX", EOrderAction.Sell, OrderExit.FromAll))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_AverageFC.price = MyBase.Bars.Close
            Me.m_AverageFC.length = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer


        ' Fields
        Private m_AverageFC As AverageFC
        Private m_Avg As VariableSeries(Of Double)
        Private m_MACrossLX As IOrderMarket
    End Class
End Namespace
