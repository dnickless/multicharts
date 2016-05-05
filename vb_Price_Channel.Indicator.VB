Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Price_Channel
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 20
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_lowerband.Value = Bars.Low.Lowest(length)
            Me.m_upperband.Value = Bars.High.Highest(length)
            m_prevLowerband.Value = m_lowerband.Item(1)
            m_prevUpperband.Value = m_upperband.Item(1)
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.Plot1.Set(Me.displace, Me.m_prevUpperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_prevLowerband.Value)
                If (Me.displace <= 0) Then
                    If Me.CrossesUnder(MyBase.Bars.Low, Me.m_prevLowerband) Then
                        MyBase.Alerts.Alert("Price making new low")
                    End If
                    If Me.CrossesOver(MyBase.Bars.High, Me.m_prevUpperband) Then
                        MyBase.Alerts.Alert("Price making new high")
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.m_prevLowerband = New VariableSeries(Of Double)(Me)
            Me.m_prevUpperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer


        ' Fields
        Private m_lowerband As VariableSeries(Of Double)
        Private m_upperband As VariableSeries(Of Double)
        Private m_prevLowerband As VariableSeries(Of Double)
        Private m_prevUpperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
