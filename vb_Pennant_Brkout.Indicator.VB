Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Pennant_Brkout
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.color2 = Color.Magenta
            Me.color1 = Color.Red
            Me.drawextensions = True
            Me.drawlines = True
            Me.barspast = 5
            Me.maxconsolindex = 1.5
            Me.length = 7
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_value1 As Integer = Me.m_pennant1.Item(0)
            Select Case m_value1
                Case 1
                    Me.m_color.Value = If((Me.m_color.Value = Me.color2), Me.color1, Me.color2)
                    If Me.drawlines Then
                        Dim mLengthminus1 As Integer = (Me.length - 1)
                        Me.m_tlhi.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item(mLengthminus1), Me.m_otlhistartpr.Value), New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_otlhiendpr.Value))
                        Me.m_tlhi.Value.ExtLeft = False
                        Me.m_tlhi.Value.ExtRight = False
                        Me.m_tlhi.Value.Color = Me.m_color.Value
                        Me.m_tllo.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item(mLengthminus1), Me.m_otllostartpr.Value), New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_otlloendpr.Value))
                        Me.m_tllo.Value.ExtLeft = False
                        Me.m_tllo.Value.ExtRight = False
                        Me.m_tllo.Value.Color = Me.m_color.Value
                    End If
                    Exit Select
                Case 2, 3
                    If Me.drawlines Then
                        Me.m_tlhi.Value.Size = 2
                        Me.m_tllo.Value.Size = 2
                    End If
                    Select Case m_value1
                        Case 2
                            Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                            Exit Select
                        Case 3
                            Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                            Exit Select
                    End Select
                    Exit Select
            End Select
            If Me.drawextensions Then
                If PublicFunctions.DoubleGreater(Me.m_otlhiendpr.Value, 0) Then
                    Me.Plot3.Set(0, Me.m_otlhiendpr.Value, Me.m_color.Value)
                End If
                If PublicFunctions.DoubleGreater(Me.m_otlloendpr.Value, 0) Then
                    Me.Plot4.Set(0, Me.m_otlloendpr.Value, Me.m_color.Value)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_pennant1 = New Pennant(Me)
            Me.m_otlhistartpr = New VariableObject(Of Double)(Me)
            Me.m_otlhiendpr = New VariableObject(Of Double)(Me)
            Me.m_otllostartpr = New VariableObject(Of Double)(Me)
            Me.m_otlloendpr = New VariableObject(Of Double)(Me)
            Me.m_color = New VariableObject(Of Color)(Me)
            Me.m_tlhi = New VariableObject(Of ITrendLineObject)(Me)
            Me.m_tllo = New VariableObject(Of ITrendLineObject)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("PenBrkoutUp", EPlotShapes.Point, Color.Cyan, Color.Empty, 5, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("PenBrkoutDn", EPlotShapes.Point, Color.Yellow, Color.Empty, 5, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("HiExt", EPlotShapes.Point, Color.Silver, Color.Empty, 3, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("LoExt", EPlotShapes.Point, Color.Silver, Color.Empty, 3, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_pennant1.length = Me.length
            Me.m_pennant1.maxconsolindex = Me.maxconsolindex
            Me.m_pennant1.barspast = Me.barspast
            Me.m_pennant1.otlhistartpr = Me.m_otlhistartpr
            Me.m_pennant1.otlhiendpr = Me.m_otlhiendpr
            Me.m_pennant1.otllostartpr = Me.m_otllostartpr
            Me.m_pennant1.otlloendpr = Me.m_otlloendpr
            Me.m_color.DefaultValue = Me.color2
        End Sub


        ' Properties
        <Input> _
        Public Property barspast As Integer

        <Input()> _
        Public Property color1 As Color

        <Input()> _
        Public Property color2 As Color

        <Input()> _
        Public Property drawextensions As Boolean

        <Input()> _
        Public Property drawlines As Boolean

        <Input()> _
        Public Property length As Integer

        <Input()> _
        Public Property maxconsolindex As Double


        ' Fields
        Private m_color As VariableObject(Of Color)
        Private m_otlhiendpr As VariableObject(Of Double)
        Private m_otlhistartpr As VariableObject(Of Double)
        Private m_otlloendpr As VariableObject(Of Double)
        Private m_otllostartpr As VariableObject(Of Double)
        Private m_pennant1 As Pennant
        Private m_tlhi As VariableObject(Of ITrendLineObject)
        Private m_tllo As VariableObject(Of ITrendLineObject)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
