Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled), CalcAtOpenNextBar(True)> _
    Public Class vb_Volty_Expan_Open_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.NumRanges = 1.2
            Me.Length = 4
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_VltOpenLE.Send((PublicFunctions.OpenNextBar(MyBase.Bars) + (Me.m_AverageFC.Item(0) * Me.NumRanges)))
        End Sub

        Protected Overrides Sub Create()
            Me.m_AverageFC = New AverageFC(Me)
            Me.m_VltOpenLE = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "VltOpnLE", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_AverageFC.price = MyBase.Bars.RangeSeries()
            Me.m_AverageFC.length = Me.Length
        End Sub


        ' Properties
        <Input> _
        Public Property Length As Integer

        <Input()> _
        Public Property NumRanges As Double


        ' Fields
        Private m_AverageFC As AverageFC
        Private m_VltOpenLE As IOrderPriced
    End Class
End Namespace
