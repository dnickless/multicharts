Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MovAvg_Cross_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 9
            Me.ConfirmBars = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_avg As Double = Me.m_AverageFC.Item(0)
            If PublicFunctions.DoubleLess(Me.Price.Item(0), m_avg) Then
                Me.m_Counter.Value = (Me.m_Counter.Value + 1)
            Else
                Me.m_Counter.Value = 0
            End If
            If (PublicFunctions.DoubleGreater(CDbl(MyBase.Bars.CurrentBar), CDbl(Me.ConfirmBars)) AndAlso (Me.m_Counter.Value = Me.ConfirmBars)) Then
                Me.m_MACrossSE.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New [Function].AverageFC(Me)
            Me.m_MACrossSE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MACrossSE", EOrderAction.SellShort))
            Me.m_Counter = New VariableObject(Of Integer)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
            Me.m_AverageFC.price = Me.Price
            Me.m_AverageFC.length = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property ConfirmBars As Integer

        <Input()> _
        Public Property Length As Integer

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_AverageFC As [Function].AverageFC
        Private m_Counter As VariableObject(Of Integer)
        Private m_MACrossSE As IOrderMarket
    End Class
End Namespace
