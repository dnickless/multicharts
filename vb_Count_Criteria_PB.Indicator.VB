Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Count_Criteria_PB
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.occur = 7
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            If (Me.criteria.CountIF(Me.length) >= Me.occur) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Plot1", EPlotShapes.BarHigh, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Plot2", EPlotShapes.BarLow, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_criteria = New Lambda(Of Boolean)(Function(_bb) PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(_bb), MyBase.Bars.High.Item((1 + _bb))))
        End Sub


        ' Properties
        Private ReadOnly Property criteria As ISeries(Of Boolean)
            Get
                Return Me.m_criteria
            End Get
        End Property

        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property occur As Integer


        ' Fields
        Private m_criteria As ISeries(Of Boolean)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
