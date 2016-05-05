Imports PowerLanguage
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_High_for_Chart
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_highesthi.Value) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert
                Me.m_highesthi.Value = MyBase.Bars.High.Item(0)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_highesthi = New VariableObject(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewHi-Ch", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_highesthi.Value = MyBase.Bars.High.Item(0)
        End Sub


        ' Fields
        Private m_highesthi As VariableObject(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
