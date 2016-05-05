Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Chaos_Gator
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.valuedown = Color.Red
            Me.valueup = Color.Green
            Me.lipoffset = 3
            Me.lip = 5
            Me.teethoffset = 5
            Me.teeth = 8
            Me.jawoffset = 8
            Me.jaw = 13
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_value1 As Double = PublicFunctions.Average(Me.value, Me.jaw, Me.jawoffset)
            Dim m_value2 As Double = PublicFunctions.Average(Me.value, Me.teeth, Me.teethoffset)
            Dim m_value3 As Double = PublicFunctions.Average(Me.value, Me.lip, Me.lipoffset)
            If (MyBase.Bars.CurrentBar >= &H15) Then
                Me.m_value11.Value = Math.Abs(CDbl((m_value1 - m_value2)))
                Me.Plot1.Set(0, Me.m_value11.Value, If(PublicFunctions.DoubleGreater(Me.m_value11.Value, Me.m_value11.Item(1)), Me.valueup, Me.valuedown))
                Me.m_value12.Value = (-1 * Math.Abs(CDbl((m_value2 - m_value3))))
                Me.Plot2.Set(0, Me.m_value12.Value, If(PublicFunctions.DoubleGreater(Me.m_value12.Value, Me.m_value12.Item(1)), Me.valueup, Me.valuedown))
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_value11 = New VariableSeries(Of Double)(Me)
            Me.m_value12 = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BLblue", EPlotShapes.Histogram, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("BLred", EPlotShapes.Histogram, Color.Red, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_value = New Lambda(Of Double)(Function(_bb) ((MyBase.Bars.High.Item(_bb) + MyBase.Bars.Low.Item(_bb)) / 2))
        End Sub


        ' Properties
        <Input> _
        Public Property jaw As Integer

        <Input()> _
        Public Property jawoffset As Integer

        <Input()> _
        Public Property lip As Integer

        <Input()> _
        Public Property lipoffset As Integer

        <Input()> _
        Public Property teeth As Integer

        <Input()> _
        Public Property teethoffset As Integer

        Private ReadOnly Property value As ISeries(Of Double)
            Get
                Return Me.m_value
            End Get
        End Property

        <Input> _
        Public Property valuedown As Color

        <Input()> _
        Public Property valueup As Color


        ' Fields
        Private m_value As ISeries(Of Double)
        Private m_value11 As VariableSeries(Of Double)
        Private m_value12 As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
