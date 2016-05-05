Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_ADX
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_adxvalue.Value = Me.m_adx1.Item(0)
            Me.Plot1.Set(0, Me.m_adxvalue.Value)
            If (PublicFunctions.DoubleGreater(Me.m_adxvalue.Value, Me.m_adxvalue.Item(1)) AndAlso PublicFunctions.DoubleLessEquals(Me.m_adxvalue.Item(1), Me.m_adxvalue.Item(2))) Then
                MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
            ElseIf (PublicFunctions.DoubleLess(Me.m_adxvalue.Value, Me.m_adxvalue.Item(1)) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_adxvalue.Item(1), Me.m_adxvalue.Item(2))) Then
                MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_adx1 = New [Function].ADX(Me)
            Me.m_adxvalue = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vb_ADX", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_adx1.Length = Me.length
            Me.m_adxvalue.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_adx1 As [Function].ADX
        Private m_adxvalue As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
