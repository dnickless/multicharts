Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    Public Class vb_MovAvg_Cross_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.Length = 9
            Me.ConfirmBars = 1
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_Avg As Double = Me.m_AverageFC.Item(0)
            If PublicFunctions.DoubleGreater(Me.Price.Item(0), m_Avg) Then
                Me.m_Counter.Value = (Me.m_Counter.Value + 1)
            Else
                Me.m_Counter.Value = 0
            End If
            If (PublicFunctions.DoubleGreater(CDbl(MyBase.Bars.CurrentBar), CDbl(Me.ConfirmBars)) AndAlso (Me.m_Counter.Value = Me.ConfirmBars)) Then
                Me.m_MACrossLE.Send()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New AverageFC(Me)
            Me.m_MACrossLE = MyBase.OrderCreator.MarketNextBar(New SOrderParameters(Contracts.Default, "MACrossLE", EOrderAction.Buy))
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
        Private m_AverageFC As AverageFC
        Private m_Counter As VariableObject(Of Integer)
        Private m_MACrossLE As IOrderMarket
    End Class
End Namespace
