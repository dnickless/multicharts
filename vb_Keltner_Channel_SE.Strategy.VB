Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Strategy
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Keltner_Channel_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumAtrs = 1.5
            Me.Length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_avg As Double = Me.m_AverageFC.Item(0)
            Dim m_shift As Double = (Me.NumAtrs * Me.AverageTrueRange(Me.Length))
            Me.m_LowerBand.Value = (m_avg - m_shift)
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesUnder(Me.Price, Me.m_LowerBand, MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_bSetupSE.Value = True
                Me.m_CrossingLow.Value = MyBase.Bars.Low.Item(0)
            ElseIf (Me.m_bSetupSE.Value AndAlso (PublicFunctions.DoubleGreater(Me.Price.Item(0), m_avg) OrElse PublicFunctions.DoubleLessEquals(MyBase.Bars.Low.Item(0), (Me.m_CrossingLow.Value - MyBase.Bars.Point)))) Then
                Me.m_bSetupSE.Value = False
            End If
            If Me.m_bSetupSE.Value Then
                Me.m_KltChSE.Send((Me.m_CrossingLow.Value - MyBase.Bars.Point))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New AverageFC(Me)
            Me.m_LowerBand = New VariableSeries(Of Double)(Me)
            Me.m_KltChSE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "KltChSE", EOrderAction.SellShort))
            Me.m_bSetupSE = New VariableObject(Of Boolean)(Me)
            Me.m_CrossingLow = New VariableObject(Of Double)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.Price = MyBase.Bars.Close
            Me.m_AverageFC.price = Me.Price
            Me.m_AverageFC.length = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property NumAtrs As Double

        Private Property Price As ISeries(Of Double)


        ' Fields
        Private m_AverageFC As AverageFC
        Private m_bSetupSE As VariableObject(Of Boolean)
        Private m_CrossingLow As VariableObject(Of Double)
        Private m_KltChSE As IOrderPriced
        Private m_LowerBand As VariableSeries(Of Double)
    End Class
End Namespace
