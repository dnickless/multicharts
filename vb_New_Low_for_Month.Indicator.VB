Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_Low_for_Month
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If (((resolution <> EResolution.Year) AndAlso (resolution <> EResolution.Quarter)) AndAlso (resolution <> EResolution.Month)) Then
                If (Bars.Time.Item(0).Month <> Bars.Time.Item(1).Month) Then
                    Me.m_lowestlo.Value = MyBase.Bars.Low.Item(0)
                ElseIf PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_lowestlo.Value) Then
                    Me.Plot1.Set(0, MyBase.Bars.Low.Item(0))
                    MyBase.Alerts.Alert()
                    Me.m_lowestlo.Value = MyBase.Bars.Low.Item(0)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_lowestlo = New VariableObject(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewLo-M", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub


        ' Fields
        Private m_lowestlo As VariableObject(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
