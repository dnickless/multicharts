Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_ADXR
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        ' Fields
        Private m_dirmovement1 As DirMovement
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject

        Protected Overrides Sub CalcBar()
            Me.m_dirmovement1.Call()
            Me.Plot1.Set(0, Me.m_dirmovement1.ADX.Value)
            Me.Plot2.Set(0, Me.m_dirmovement1.ADXR.Value)
            If (PublicFunctions.DoubleGreater(Me.m_dirmovement1.ADXR.Value, Me.m_dirmovement1.ADXR.Item(1)) AndAlso PublicFunctions.DoubleLessEquals(Me.m_dirmovement1.ADXR.Item(1), Me.m_dirmovement1.ADXR.Item(2))) Then
                MyBase.Alerts.Alert("vb_ADXR turning up", New Object(0 - 1) {})
            ElseIf (PublicFunctions.DoubleLess(Me.m_dirmovement1.ADXR.Value, Me.m_dirmovement1.ADXR.Item(1)) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_dirmovement1.ADXR.Item(1), Me.m_dirmovement1.ADXR.Item(2))) Then
                MyBase.Alerts.Alert("vb_ADXR turning down", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_dirmovement1 = New DirMovement(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("ADX", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("vb_ADXR", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_dirmovement1.PriceH = MyBase.Bars.High
            Me.m_dirmovement1.PriceL = MyBase.Bars.Low
            Me.m_dirmovement1.PriceC = MyBase.Bars.Close
            Me.m_dirmovement1.Length = Me.length
        End Sub


        ' Properties
        <Input()> _
        Public Property length As Integer

    End Class
End Namespace
