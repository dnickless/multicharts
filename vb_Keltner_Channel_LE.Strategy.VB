Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Keltner_Channel_LE
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
            Me.m_UpperBand.Value = (m_avg + m_shift)
            If ((MyBase.Bars.CurrentBar > 1) AndAlso PublicFunctions.CrossesOver(Me.Price, Me.m_UpperBand, MyBase.ExecInfo.MaxBarsBack)) Then
                Me.m_bSetupLE.Value = True
                Me.m_CrossingHigh.Value = MyBase.Bars.High.Item(0)
            ElseIf (Me.m_bSetupLE.Value AndAlso (PublicFunctions.DoubleLess(Me.Price.Item(0), m_avg) OrElse PublicFunctions.DoubleGreaterEquals(MyBase.Bars.High.Item(0), (Me.m_CrossingHigh.Value + MyBase.Bars.Point)))) Then
                Me.m_bSetupLE.Value = False
            End If
            If Me.m_bSetupLE.Value Then
                Me.m_KltChLE.Send((Me.m_CrossingHigh.Value + MyBase.Bars.Point))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New AverageFC(Me)
            Me.m_UpperBand = New VariableSeries(Of Double)(Me)
            Me.m_bSetupLE = New VariableObject(Of Boolean)(Me)
            Me.m_CrossingHigh = New VariableObject(Of Double)(Me)
            Me.m_KltChLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "KltChLE", EOrderAction.Buy))
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
        Private m_bSetupLE As VariableObject(Of Boolean)
        Private m_CrossingHigh As VariableObject(Of Double)
        Private m_KltChLE As IOrderPriced
        Private m_UpperBand As VariableSeries(Of Double)
    End Class
End Namespace
