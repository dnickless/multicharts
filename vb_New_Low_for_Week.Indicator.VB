Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_Low_for_Week
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((resolution <> EResolution.Quarter) AndAlso ((EResolution.Week > resolution) OrElse (resolution > EResolution.Year))) Then
                If (Bars.Time.Item(0).DayOfWeek < Bars.Time.Item(1).DayOfWeek) Then
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
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewLo-W", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub


        ' Fields
        Private m_lowestlo As VariableObject(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
