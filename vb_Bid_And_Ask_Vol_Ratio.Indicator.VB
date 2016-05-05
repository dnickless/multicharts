Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Bid_And_Ask_Vol_Ratio
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.uselog = True
            Me.datanum = 1
        End Sub

        Protected Overrides Sub CalcBar()
            If (MyBase.Bars.LastBarOnChart AndAlso (MyBase.Bars.Status <> EBarState.Close)) Then
                Dim m_myvol As Double = MyBase.Bars.TrueVolume().Item(0)
                If (MyBase.Bars.CurrentBar > Me.m_mycurrentbar.Value) Then
                    Me.m_volumeatbid.Value = 0
                    Me.m_volumeatask.Value = 0
                    Me.m_bavolratio.Value = 0
                    Me.m_voltmp.Value = 0
                    Me.m_mycurrentbar.Value = MyBase.Bars.CurrentBar
                End If
                If PublicFunctions.DoubleLess(MyBase.Bars.StatusLine.Bid, MyBase.Bars.StatusLine.Ask) Then
                    If PublicFunctions.DoubleLessEquals(MyBase.Bars.Close.Item(0), MyBase.Bars.StatusLine.Bid) Then
                        Me.m_volumeatbid.Value = ((Me.m_volumeatbid.Value + m_myvol) - Me.m_voltmp.Value)
                    ElseIf PublicFunctions.DoubleGreaterEquals(MyBase.Bars.Close.Item(0), MyBase.Bars.StatusLine.Ask) Then
                        Me.m_volumeatask.Value = ((Me.m_volumeatask.Value + m_myvol) - Me.m_voltmp.Value)
                    End If
                End If
                If (PublicFunctions.DoubleGreater(Me.m_volumeatbid.Value, 0) AndAlso PublicFunctions.DoubleGreater(Me.m_volumeatask.Value, 0)) Then
                    Me.m_bavolratio.Value = PublicFunctions.Iff(Of Double)(Me.uselog, Math.Log((Me.m_volumeatask.Value / Me.m_volumeatbid.Value)), (Me.m_volumeatask.Value / Me.m_volumeatbid.Value))
                End If
                Me.m_voltmp.Value = m_myvol
                Me.Plot1.Set(0, Me.m_bavolratio.Value)
            End If
            Me.Plot2.Set(0, 0)
        End Sub

        Protected Overrides Sub Create()
            Me.m_mycurrentbar = New VariableSeries(Of Integer)(Me, 0, 0, True)
            Me.m_volumeatbid = New VariableSeries(Of Double)(Me, 0, 0, True)
            Me.m_volumeatask = New VariableSeries(Of Double)(Me, 0, 0, True)
            Me.m_bavolratio = New VariableSeries(Of Double)(Me, 0, 0, True)
            Me.m_voltmp = New VariableSeries(Of Double)(Me, 0, 0, True)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BAVolRatio", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property datanum As Double

        <Input()> _
        Public Property uselog As Boolean

        ' Fields
        Private m_bavolratio As VariableSeries(Of Double)
        Private m_mycurrentbar As VariableSeries(Of Integer)
        Private m_voltmp As VariableSeries(Of Double)
        Private m_volumeatask As VariableSeries(Of Double)
        Private m_volumeatbid As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
