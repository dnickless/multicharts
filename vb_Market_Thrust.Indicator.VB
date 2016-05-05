Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Market_Thrust
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.smoothinglength = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_rawmktthr.Value = ((Me.advissues.Item(0) * Me.advvol.Item(0)) - (Me.decissues.Item(0) * Me.decvol.Item(0)))
            Me.m_mktthr.Value = PublicFunctions.Average(Me.m_rawmktthr, Me.smoothinglength)
            Me.Plot1.Set(0, Me.m_rawmktthr.Value)
            Me.Plot2.Set(0, Me.m_mktthr.Value)
            Me.Plot3.Set(0, 0)
            If DirectCast(Me, IStudy).CrossesOver(DirectCast(Me.m_mktthr, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning positive", New Object(0 - 1) {})
            ElseIf DirectCast(Me, IStudy).CrossesUnder(DirectCast(Me.m_mktthr, ISeries(Of Double)), CDbl(0)) Then
                MyBase.Alerts.Alert("Indicator turning negative", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_rawmktthr = New VariableSeries(Of Double)(Me)
            Me.m_mktthr = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("RawMktThr", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("MktThr", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("ZeroLine", EPlotShapes.Line, Color.Green, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_advissues = MyBase.Bars.Close
            Me.m_advvol = MyBase.BarsOfData(2).Close
            Me.m_decissues = MyBase.BarsOfData(3).Close
            Me.m_decvol = MyBase.BarsOfData(4).Close
        End Sub


        ' Properties
        Private ReadOnly Property advissues As ISeries(Of Double)
            Get
                Return Me.m_advissues
            End Get
        End Property

        Private ReadOnly Property advvol As ISeries(Of Double)
            Get
                Return Me.m_advvol
            End Get
        End Property

        Private ReadOnly Property decissues As ISeries(Of Double)
            Get
                Return Me.m_decissues
            End Get
        End Property

        Private ReadOnly Property decvol As ISeries(Of Double)
            Get
                Return Me.m_decvol
            End Get
        End Property

        <Input> _
        Public Property smoothinglength As Integer


        ' Fields
        Private m_advissues As ISeries(Of Double)
        Private m_advvol As ISeries(Of Double)
        Private m_decissues As ISeries(Of Double)
        Private m_decvol As ISeries(Of Double)
        Private m_mktthr As VariableSeries(Of Double)
        Private m_rawmktthr As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
