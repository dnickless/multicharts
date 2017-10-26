Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Key_Reversal_Up
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 1
        End Sub

        Protected Overrides Sub CalcBar()
            m_lowest.Value = Bars.Low.Lowest(length)
            If (PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), m_lowest.Item(1)) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(0), MyBase.Bars.Close.Item(1))) Then
                Me.Plot1.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert()
            Else
                Me.Plot1.Reset()
            End If
        End Sub

        Protected Overrides Sub Create()
            m_lowest = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("KeyRevUp", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_lowest As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
