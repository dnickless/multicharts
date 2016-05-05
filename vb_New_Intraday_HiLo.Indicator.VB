Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_New_Intraday_HiLo
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.endbartime = New DateTime(1, 1, 1, 14, 30, 0)
            Me.startbartime = New DateTime(1, 1, 1, 10, 0, 0)
            Me.barsbetween = 10
            Me.requirereversalgap = True
            Me.numdays = 3
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_rs_dailydataarray1.Call
            Dim m_barnum As Integer = Me.m_barnumber1.Item(0)
            Dim m_barssincedayhi As Double = (m_barnum - Me.m_dataarray.Item(0)(10, Me.m_index.Value))
            Dim m_barssincedaylo As Double = (m_barnum - Me.m_dataarray.Item(0)(11, Me.m_index.Value))
            Dim mBbminus1 As Double = (Me.barsbetween - 1)
            Me.m_qualhi.Value = PublicFunctions.DoubleGreaterEquals(m_barssincedayhi, mBbminus1)
            Me.m_quallo.Value = PublicFunctions.DoubleGreaterEquals(m_barssincedaylo, mBbminus1)
            If Me.requirereversalgap Then
                If ((MyBase.Bars.CurrentBar = 1) OrElse (Bars.Time.Item(1).Date <> Bars.Time.Item(0).Date)) Then
                    Me.m_rs_extremes1.Call()
                    Dim m_gapup As Boolean = PublicFunctions.DoubleGreater(Me.m_dataarray.Item(0)(1, Me.m_index.Value), Me.m_oprevhighest.Value)
                    Dim m_gapdn As Boolean = PublicFunctions.DoubleLess(Me.m_dataarray.Item(0)(1, Me.m_index.Value), Me.m_oprevlowest.Value)
                    Dim m_thirdprevavg As Double = Me.m_rs_average1.Item(0)
                    Dim m_secondprevavg As Double = Me.m_rs_average2.Item(0)
                    Dim m_prevavg As Double = Me.m_rs_average3.Item(0)
                    Dim m_uptrend As Boolean = (PublicFunctions.DoubleGreater(m_prevavg, m_secondprevavg) AndAlso PublicFunctions.DoubleGreater(m_secondprevavg, m_thirdprevavg))
                    Dim m_dntrend As Boolean = (PublicFunctions.DoubleLess(m_prevavg, m_secondprevavg) AndAlso PublicFunctions.DoubleLess(m_secondprevavg, m_thirdprevavg))
                    Me.m_revgapup.Value = (m_dntrend AndAlso m_gapup)
                    Me.m_revgapdn.Value = (m_uptrend AndAlso m_gapdn)
                End If
            Else
                Me.m_revgapup.Value = True
                Me.m_revgapdn.Value = True
            End If
            If (Bars.Time.Item(0).Date = Bars.Time.Item(1).Date) Then
                Dim m_newdayhi As Boolean = If(((m_barssincedayhi <> 0) OrElse Not Me.m_qualhi.Item(1)), False, Me.m_revgapup.Value)
                Dim m_newdaylo As Boolean = If(((m_barssincedaylo <> 0) OrElse Not Me.m_quallo.Item(1)), False, Me.m_revgapdn.Value)
                If (m_newdayhi AndAlso m_newdaylo) Then
                    Me.Plot1.Set(0, MyBase.Bars.Close.Item(0), Color.White)
                ElseIf m_newdayhi Then
                    Me.Plot1.Set(0, MyBase.Bars.High.Item(0), Color.Yellow)
                ElseIf m_newdaylo Then
                    Me.Plot1.Set(0, MyBase.Bars.Low.Item(0), Color.Cyan)
                End If
            End If
            Dim _now_time As New DateTime(1, 1, 1, Bars.Time.Item(0).Hour, Bars.Time.Item(0).Minute, Bars.Time.Item(0).Second)
            If ((_now_time < Me.startbartime) OrElse (_now_time > Me.endbartime)) Then
                Me.Plot1.Colors.Item(0) = Color.DarkGray
            End If
            If Me.requirereversalgap Then
                If Me.m_revgapup.Value Then
                    Me.Plot2.Set(0, MyBase.Bars.Close.Item(0), Color.Blue)
                ElseIf Me.m_revgapdn.Value Then
                    Me.Plot2.Set(0, MyBase.Bars.Close.Item(0), New Color)
                End If
            End If
            Me.Plot3.Set(0, Me.m_dataarray.Item(0)(2, Me.m_index.Value))
            Me.Plot4.Set(0, Me.m_dataarray.Item(0)(3, Me.m_index.Value))
            If Me.m_qualhi.Value Then
                Me.Plot3.Widths.Item(0) = 2
            End If
            If Me.m_quallo.Value Then
                Me.Plot4.Widths.Item(0) = 2
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_rs_dailydataarray1 = New [Function].RS_DailyDataArray(Me)
            Me.m_barnumber1 = New [Function].BarNumber(Me)
            Me.m_rs_extremes1 = New [Function].RS_Extremes(Me)
            Me.m_rs_average1 = New [Function].RS_Average(Me)
            Me.m_rs_average2 = New [Function].RS_Average(Me)
            Me.m_rs_average3 = New [Function].RS_Average(Me)
            Me.m_index = New VariableObject(Of Integer)(Me)
            Me.m_qualhi = New VariableSeries(Of Boolean)(Me)
            Me.m_quallo = New VariableSeries(Of Boolean)(Me)
            Me.m_oprevhighest = New VariableObject(Of Double)(Me)
            Me.m_oprevhighestday = New VariableObject(Of Double)(Me)
            Me.m_oprevlowest = New VariableObject(Of Double)(Me)
            Me.m_oprevlowestday = New VariableObject(Of Double)(Me)
            Me.m_revgapup = New VariableObject(Of Boolean)(Me)
            Me.m_revgapdn = New VariableObject(Of Boolean)(Me)
            Me.m_dataarray = New Array2DSimple(Of Double)(Me, 13, &H65)
            Me.m_subarray = New ArraySimple(Of Double)(Me, 4)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("NewHiLo", EPlotShapes.Point, Color.Cyan, Color.Empty, 5, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("RevGap", EPlotShapes.Point, Color.Yellow, Color.Empty, 2, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("HiLine", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("LoLine", EPlotShapes.Line, Color.Magenta, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_rs_dailydataarray1.numdays = (Me.numdays + 2)
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
            Me.m_rs_average1.rowtoavg = 6
            Me.m_rs_average1.numdays = Me.numdays
            Me.m_rs_average1.offset = 2
            Me.m_rs_average1.dataarray = Me.m_dataarray
            Me.m_rs_average1.index = Me.m_index
            Me.m_rs_average2.rowtoavg = 6
            Me.m_rs_average2.numdays = Me.numdays
            Me.m_rs_average2.offset = 1
            Me.m_rs_average2.dataarray = Me.m_dataarray
            Me.m_rs_average2.index = Me.m_index
            Me.m_rs_average3.rowtoavg = 6
            Me.m_rs_average3.numdays = Me.numdays
            Me.m_rs_average3.offset = 0
            Me.m_rs_average3.dataarray = Me.m_dataarray
            Me.m_rs_average3.index = Me.m_index
            Me.m_dataarray.DefaultValue = 0
            Me.m_subarray.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property barsbetween As Double

        <Input()> _
        Public Property endbartime As DateTime

        <Input()> _
        Public Property numdays As Integer

        <Input()> _
        Public Property requirereversalgap As Boolean

        <Input()> _
        Public Property startbartime As DateTime


        ' Fields
        Private m_barnumber1 As [Function].BarNumber
        Private m_dataarray As Array2DSimple(Of Double)
        Private m_index As VariableObject(Of Integer)
        Private m_oprevhighest As VariableObject(Of Double)
        Private m_oprevhighestday As VariableObject(Of Double)
        Private m_oprevlowest As VariableObject(Of Double)
        Private m_oprevlowestday As VariableObject(Of Double)
        Private m_qualhi As VariableSeries(Of Boolean)
        Private m_quallo As VariableSeries(Of Boolean)
        Private m_revgapdn As VariableObject(Of Boolean)
        Private m_revgapup As VariableObject(Of Boolean)
        Private m_rs_average1 As [Function].RS_Average
        Private m_rs_average2 As [Function].RS_Average
        Private m_rs_average3 As [Function].RS_Average
        Private m_rs_dailydataarray1 As [Function].RS_DailyDataArray
        Private m_rs_extremes1 As [Function].RS_Extremes
        Private m_subarray As ArraySimple(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
