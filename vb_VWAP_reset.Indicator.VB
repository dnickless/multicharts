Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_VWAP_reset
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.dncolor = Color.Magenta
            Me.upcolor = Color.Cyan
            Me.time5 = New DateTime(1, 1, 1, 14, 15, 0)
            Me.time4 = New DateTime(1, 1, 1, 13, 15, 0)
            Me.time3 = New DateTime(1, 1, 1, 11, &H1D, 0)
            Me.time2 = New DateTime(1, 1, 1, 9, 30, 0)
            Me.time1 = New DateTime(1, 1, 1, 9, 0, 0)
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_price.Value = MyBase.Bars.AvgPrice(0)
            Me.m_it.Value = Me.m_vwapresettable1.Item(0)
            Me.Plot1.Set(0, Me.m_it.Value)
            If PublicFunctions.DoubleGreater(Me.m_it.Value, Me.m_it.Item(1)) Then
                Me.m_color.Value = Me.upcolor
            End If
            If PublicFunctions.DoubleLess(Me.m_it.Value, Me.m_it.Item(1)) Then
                Me.m_color.Value = Me.dncolor
            End If
            Me.Plot1.Colors.Item(0) = Me.m_color.Value
        End Sub

        Protected Overrides Sub Create()
            Me.m_vwapresettable1 = New vwapResettable(Me)
            Me.m_price = New VariableSeries(Of Double)(Me)
            Me.m_it = New VariableSeries(Of Double)(Me)
            Me.m_color = New VariableObject(Of Color)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("vwap_reset", EPlotShapes.Point, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_vwapresettable1.price = Me.m_price
            Me.m_vwapresettable1.localhrsoffset = Me.localhrsoffset
            Me.m_vwapresettable1.time1 = Me.time1
            Me.m_vwapresettable1.time2 = Me.time2
            Me.m_vwapresettable1.time3 = Me.time3
            Me.m_vwapresettable1.time4 = Me.time4
            Me.m_vwapresettable1.time5 = Me.time5
            Me.m_color.DefaultValue = Color.Yellow
        End Sub


        ' Properties
        <Input> _
        Public Property dncolor As Color

        <Input()> _
        Public Property localhrsoffset As Integer

        <Input()> _
        Public Property time1 As DateTime

        <Input()> _
        Public Property time2 As DateTime

        <Input()> _
        Public Property time3 As DateTime

        <Input()> _
        Public Property time4 As DateTime

        <Input()> _
        Public Property time5 As DateTime

        <Input()> _
        Public Property upcolor As Color


        ' Fields
        Private m_color As VariableObject(Of Color)
        Private m_it As VariableSeries(Of Double)
        Private m_price As VariableSeries(Of Double)
        Private m_vwapresettable1 As vwapResettable
        Private Plot1 As IPlotObject
    End Class
End Namespace
