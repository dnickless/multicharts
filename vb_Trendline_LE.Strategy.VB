Imports PowerLanguage
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled), CalcAtOpenNextBar(True)> _
    Public Class vb_Trendline_LE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (((Not Me.tlRef Is Nothing) AndAlso Me.tlRef.Exist) AndAlso ((MyBase.Bars.Time.Item(0) >= Me.m_BeginTime) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.High.Item(0), Me.tlRef.PriceValue(MyBase.Bars.Time.Item(0))))) Then
                Me.m_BuyOrder.Send(Me.tlRef.PriceValue(PublicFunctions.TimeNextBar(MyBase.Bars)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_BuyOrder = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Buy", EOrderAction.Buy))
        End Sub

        Protected Overrides Sub StartCalc()
            Dim lines As IEnumerable(Of ITrendLineObject) = MyBase.DrwTrendLine.GetTrendLineObjects(EDrawingSource.AnyTechOrManual)
            Dim drw As ITrendLineObject
            For Each drw In lines
                If (drw.ID = Me.TlID) Then
                    Me.tlRef = drw
                    Me.m_BeginTime = Me.tlRef.Begin.Time
                End If
            Next
        End Sub


        ' Properties
        <Input> _
        Public Property TlID As Integer


        ' Fields
        Private m_BeginTime As DateTime
        Private m_BuyOrder As IOrderPriced
        Private tlRef As ITrendLineObject
    End Class
End Namespace
