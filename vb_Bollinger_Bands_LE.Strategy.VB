Imports PowerLanguage
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_Bollinger_Bands_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 20
            Me.NumDevsDn = 2
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_LowerBand.Value = MyBase.Bars.Close.BollingerBandCustom(Me.Length, CDbl(-Me.NumDevsDn))
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesOver(MyBase.Bars.Close, Me.m_LowerBand, MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_BBandLE.Send(Me.m_LowerBand.Value)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_LowerBand = New VariableSeries(Of Double)(Me)
            Me.m_BBandLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "BBandLE", EOrderAction.Buy))
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property NumDevsDn As Double


        ' Fields
        Private m_BBandLE As IOrderPriced
        Private m_LowerBand As VariableSeries(Of Double)
    End Class
End Namespace
