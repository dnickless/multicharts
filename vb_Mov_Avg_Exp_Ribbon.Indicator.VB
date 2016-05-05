Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Mov_Avg_Exp_Ribbon
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.slowcolor = Color.Red
            Me.fastcolor = Color.Yellow
            Me.incrementormultiplier = 10
            Me.arithorgeom_1or2 = 1
            Me.baselength = 10
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_counter As Integer
            If (MyBase.Bars.CurrentBar = 1) Then
                Me.m_length.Item(0)(1) = Me.baselength
                Me.m_smoothingfactor.Item(0)(1) = (2 / CDbl((Me.m_length.Item(0)(1) + 1)))
                Me.m_xma.Item(0)(1) = Me.price.Item(0)
                Me.m_color.Item(0)(1) = PublicFunctions.GradientColor(1, 1, 8, Me.fastcolor, Me.slowcolor)
                m_counter = 1
                Do While (m_counter <= 7)
                    If (Me.arithorgeom_1or2 = 1) Then
                        Me.m_length.Item(0)((m_counter + 1)) = (Me.m_length.Item(0)(m_counter) + Me.incrementormultiplier)
                    Else
                        Me.m_length.Item(0)((m_counter + 1)) = (Me.m_length.Item(0)(m_counter) * Me.incrementormultiplier)
                    End If
                    Me.m_smoothingfactor.Item(0)((m_counter + 1)) = (2 / CDbl((Me.m_length.Item(0)((m_counter + 1)) + 1)))
                    Me.m_xma.Item(0)((m_counter + 1)) = Me.price.Item(0)
                    Me.m_color.Item(0)((m_counter + 1)) = PublicFunctions.GradientColor(CDbl((m_counter + 1)), 1, 8, Me.fastcolor, Me.slowcolor)
                    m_counter += 1
                Loop
            Else
                m_counter = 1
                Do While (m_counter <= 8)
                    Me.m_xma.Item(0)(m_counter) = (Me.m_xma.Item(1)(m_counter) + (Me.m_smoothingfactor.Item(0)(m_counter) * (Me.price.Item(0) - Me.m_xma.Item(1)(m_counter))))
                    m_counter += 1
                Loop
            End If
            Me.Plot1.Set(0, Me.m_xma.Item(0)(1), Me.m_color.Item(0)(1))
            Me.Plot2.Set(0, Me.m_xma.Item(0)(2), Me.m_color.Item(0)(2))
            Me.Plot3.Set(0, Me.m_xma.Item(0)(3), Me.m_color.Item(0)(3))
            Me.Plot4.Set(0, Me.m_xma.Item(0)(4), Me.m_color.Item(0)(4))
            Me.Plot5.Set(0, Me.m_xma.Item(0)(5), Me.m_color.Item(0)(5))
            Me.Plot6.Set(0, Me.m_xma.Item(0)(6), Me.m_color.Item(0)(6))
            Me.Plot7.Set(0, Me.m_xma.Item(0)(7), Me.m_color.Item(0)(7))
            Me.Plot8.Set(0, Me.m_xma.Item(0)(8), Me.m_color.Item(0)(8))
        End Sub

        Protected Overrides Sub Create()
            Me.m_length = New ArraySimple(Of Integer)(Me, 9)
            Me.m_smoothingfactor = New ArraySimple(Of Double)(Me, 9)
            Me.m_xma = New ArraySeries(Of Double)(Me, 9)
            Me.m_color = New ArraySimple(Of Color)(Me, 9)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("XMA1", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("XMA2", EPlotShapes.Line, Color.Red, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("XMA3", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("XMA4", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot5 = MyBase.AddPlot(New PlotAttributes("XMA5", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot6 = MyBase.AddPlot(New PlotAttributes("XMA6", EPlotShapes.Line, Color.Red, Color.Empty, 0, 0, True))
            Me.Plot7 = MyBase.AddPlot(New PlotAttributes("XMA7", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
            Me.Plot8 = MyBase.AddPlot(New PlotAttributes("XMA8", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
        End Sub


        ' Properties
        <Input> _
        Public Property arithorgeom_1or2 As Integer

        <Input()> _
        Public Property baselength As Integer

        <Input()> _
        Public Property fastcolor As Color

        <Input()> _
        Public Property incrementormultiplier As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property slowcolor As Color


        ' Fields
        Private m_color As ArraySimple(Of Color)
        Private m_length As ArraySimple(Of Integer)
        Private m_smoothingfactor As ArraySimple(Of Double)
        Private m_xma As ArraySeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
        Private Plot5 As IPlotObject
        Private Plot6 As IPlotObject
        Private Plot7 As IPlotObject
        Private Plot8 As IPlotObject
    End Class
End Namespace
