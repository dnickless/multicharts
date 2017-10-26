Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Chaos_Alligator
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.lipoffset = 3
            Me.lip = 5
            Me.teethoffset = 5
            Me.teeth = 8
            Me.jawoffset = 8
            Me.jaw = 13
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_value1 As Double = PublicFunctions.Average(Me.m_avg_price, Me.jaw, Me.jawoffset)
            Dim m_value2 As Double = PublicFunctions.Average(Me.m_avg_price, Me.teeth, Me.teethoffset)
            Dim m_value3 As Double = PublicFunctions.Average(Me.m_avg_price, Me.lip, Me.lipoffset)
            If (MyBase.Bars.CurrentBar >= &H15) Then
                Me.Plot1.Set(0, m_value1)
                Me.Plot2.Set(0, m_value2)
                Me.Plot3.Set(0, m_value3)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BLblue", EPlotShapes.Line, Color.Red, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("BLred", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("BLgreen", EPlotShapes.Line, Color.Lime, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_avg_price = New Lambda(Of Double)(Function(_bb) ((MyBase.Bars.High.Item(_bb) + MyBase.Bars.Low.Item(_bb)) / 2))
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


        ' Fields
        Private m_avg_price As ISeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
