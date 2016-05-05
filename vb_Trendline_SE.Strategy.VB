Imports PowerLanguage
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Strategy
    <CalcAtOpenNextBar(True), IOGMode(IOGMode.Disabled)> _
    Public Class vb_Trendline_SE
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If (((Not Me.tlref Is Nothing) AndAlso Me.tlref.Exist) AndAlso ((MyBase.Bars.Time.Item(0) >= Me.m_BeginTime) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Low.Item(0), Me.tlref.PriceValue(MyBase.Bars.Time.Item(0))))) Then
                Me.m_ShortOrder.Send(Me.tlref.PriceValue(PublicFunctions.TimeNextBar(MyBase.Bars)))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_ShortOrder = MyBase.OrderCreator.Stop(New SOrderParameters(Contracts.Default, "Short", EOrderAction.SellShort))
        End Sub

        Protected Overrides Sub StartCalc()
            Dim lines As IEnumerable(Of ITrendLineObject) = MyBase.DrwTrendLine.GetTrendLineObjects(EDrawingSource.AnyTechOrManual)
            Dim drw As ITrendLineObject
            For Each drw In lines
                If (drw.ID = Me.tlID) Then
                    Me.tlref = drw
                    Me.m_BeginTime = Me.tlref.Begin.Time
                End If
            Next
        End Sub


        ' Properties
        <Input> _
        Public Property tlID As Integer


        ' Fields
        Private m_BeginTime As DateTime
        Private m_ShortOrder As IOrderPriced
        Private tlref As ITrendLineObject
    End Class
End Namespace
