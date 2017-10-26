Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Bollinger_Bands_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumDevsUp = 2
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_HigherBand.Value = MyBase.Bars.Close.BollingerBandCustom(Me.Length, CDbl(Me.NumDevsUp))
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesUnder(MyBase.Bars.Close, Me.m_HigherBand, MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_BBandSE.Send(Me.m_HigherBand.Value)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_HigherBand = New VariableSeries(Of Double)(Me)
            Me.m_BBandSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "BBandSE", EOrderAction.SellShort))
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property NumDevsUp As Double


        ' Fields
        Private m_BBandSE As IOrderPriced
        Private m_HigherBand As VariableSeries(Of Double)
    End Class
End Namespace
