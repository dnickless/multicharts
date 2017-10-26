Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_NDay_HiLo
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.numdays = 7
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_rs_dailydataarray1.Call()
            If ((MyBase.Bars.CurrentBar = 1) OrElse (MyBase.Bars.Time.Item(1).Date <> Bars.Time.Item(0).Date)) Then
                Me.m_rs_extremes1.Call()
                Dim m_startday As Integer = (Me.numdays - Me.oldqualdays)
                Me.m_qualhi.Value = (PublicFunctions.DoubleLessEquals(Me.m_oprevhighestday.Value, CDbl(m_startday)) AndAlso PublicFunctions.DoubleGreater(Me.m_oprevhighestday.Value, CDbl(Me.newqualdays)))
                Me.m_quallo.Value = (PublicFunctions.DoubleLessEquals(Me.m_oprevlowestday.Value, CDbl(m_startday)) AndAlso PublicFunctions.DoubleGreater(Me.m_oprevlowestday.Value, CDbl(Me.newqualdays)))
            End If
            If Not If((Not Me.m_qualhi.Value OrElse Not PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_oprevhighest.Value)), True, Not PublicFunctions.IffLogic(Of Boolean)((Me.m_dataarray.Item(0)(12, Me.m_index.Value) = Me.m_subarray.Item(0)(3)), PublicFunctions.DoubleLessEquals(Me.m_subarray.Item(0)(1), Me.m_oprevhighest.Value), True)) Then
                If (Not MyBase.Bars.Time.Item(0) = Me.m_prevnewhidate.Value) Then
                    Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                End If
                Me.m_prevnewhidate.Value = MyBase.Bars.Time.Item(0)
            ElseIf Not If((Not Me.m_quallo.Value OrElse Not PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_oprevlowest.Value)), True, Not PublicFunctions.IffLogic(Of Boolean)((Me.m_dataarray.Item(0)(12, Me.m_index.Value) = Me.m_subarray.Item(0)(3)), PublicFunctions.DoubleGreaterEquals(Me.m_subarray.Item(0)(2), Me.m_oprevlowest.Value), True)) Then
                If (Not MyBase.Bars.Time.Item(0) = Me.m_prevnewlodate.Value) Then
                    Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                End If
                Me.m_prevnewlodate.Value = MyBase.Bars.Time.Item(0)
            End If
            Me.Plot3.Set(0, Me.m_oprevhighest.Value)
            Me.Plot4.Set(0, Me.m_oprevlowest.Value)
            If Me.m_qualhi.Value Then
                Me.Plot3.Widths.Item(0) = 2
            End If
            If Me.m_quallo.Value Then
                Me.Plot4.Widths.Item(0) = 2
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_rs_dailydataarray1 = New [Function].RS_DailyDataArray(Me)
            Me.m_rs_extremes1 = New [Function].RS_Extremes(Me)
            Me.m_index = New VariableObject(Of Integer)(Me)
            Me.m_oprevhighest = New VariableObject(Of Double)(Me)
            Me.m_oprevhighestday = New VariableObject(Of Double)(Me)
            Me.m_oprevlowest = New VariableObject(Of Double)(Me)
            Me.m_oprevlowestday = New VariableObject(Of Double)(Me)
            Me.m_qualhi = New VariableObject(Of Boolean)(Me)
            Me.m_quallo = New VariableObject(Of Boolean)(Me)
            Me.m_prevnewhidate = New VariableObject(Of DateTime)(Me)
            Me.m_prevnewlodate = New VariableObject(Of DateTime)(Me)
            Me.m_dataarray = New Array2DSimple(Of Double)(Me, 13, &H65)
            Me.m_subarray = New ArraySimple(Of Double)(Me, 4)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewNDayHi", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("NewNDayLo", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("HiLine", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("LoLine", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_rs_dailydataarray1.numdays = Me.numdays
            Me.m_rs_dailydataarray1.odataarray = Me.m_dataarray
            Me.m_rs_dailydataarray1.oindex = Me.m_index
            Me.m_rs_dailydataarray1.osubarray = Me.m_subarray
            Me.m_rs_extremes1.numdays = Me.numdays
            Me.m_rs_extremes1.dataarray = Me.m_dataarray
            Me.m_rs_extremes1.index = Me.m_index
            Me.m_rs_extremes1.oprevhighest = Me.m_oprevhighest
            Me.m_rs_extremes1.oprevhighestday = Me.m_oprevhighestday
            Me.m_rs_extremes1.oprevlowest = Me.m_oprevlowest
            Me.m_rs_extremes1.oprevlowestday = Me.m_oprevlowestday
            Me.m_prevnewhidate.DefaultValue = DateTime.MinValue
            Me.m_prevnewlodate.DefaultValue = DateTime.MinValue
        End Sub


        ' Properties
        <Input> _
        Public Property newqualdays As Integer

        <Input()> _
        Public Property numdays As Integer

        <Input()> _
        Public Property oldqualdays As Integer


        ' Fields
        Private m_dataarray As Array2DSimple(Of Double)
        Private m_index As VariableObject(Of Integer)
        Private m_oprevhighest As VariableObject(Of Double)
        Private m_oprevhighestday As VariableObject(Of Double)
        Private m_oprevlowest As VariableObject(Of Double)
        Private m_oprevlowestday As VariableObject(Of Double)
        Private m_prevnewhidate As VariableObject(Of DateTime)
        Private m_prevnewlodate As VariableObject(Of DateTime)
        Private m_qualhi As VariableObject(Of Boolean)
        Private m_quallo As VariableObject(Of Boolean)
        Private m_rs_dailydataarray1 As [Function].RS_DailyDataArray
        Private m_rs_extremes1 As [Function].RS_Extremes
        Private m_subarray As ArraySimple(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
