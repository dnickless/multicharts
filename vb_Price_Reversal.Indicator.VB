Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Price_Reversal
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.lolinecolor = Color.LightGray
            Me.hilinecolor = Color.Magenta
            Me.gapsizefactor = 0.2
            Me.revcriteria = 1
            Me.finalrangefactor = 1
            Me.minconsolindex = 2.25
            Me.numdays = 3
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_rs_dailydataarray1.Call()
            If ((MyBase.Bars.CurrentBar = 1) OrElse (Bars.Time.Item(1).Date <> Bars.Time.Item(0).Date)) Then
                Me.m_rs_trueextremes1.Call()
                Me.m_value3.Value = Me.m_rs_priceextension1.Item(0)
            End If
            Me.m_rs_reversalpatterns1.Call
            If (Me.m_value3.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                If (Me.m_osetup.Value = 1) Then
                    Me.Plot2.Set(0, MyBase.Bars.Close.Item(0))
                    If PublicFunctions.DoubleGreater(Me.m_otrigger.Value, 0) Then
                        Me.Plot3.Set(0, MyBase.Bars.Low.Item(0))
                    End If
                End If
            ElseIf (Me.m_value3.Value = 2) Then
                Me.Plot1.Set(0, MyBase.Bars.Low.Item(0))
                If (Me.m_osetup.Value = 1) Then
                    Me.Plot2.Set(0, MyBase.Bars.Close.Item(0))
                    If PublicFunctions.DoubleGreater(Me.m_otrigger.Value, 0) Then
                        Me.Plot3.Set(0, MyBase.Bars.High.Item(0))
                    End If
                End If
            End If
            If ((Me.m_barnumber1.Item(0) Mod 2) = 0) Then
                Me.Plot4.Set(0, Me.m_prevtrhighest.Value, Me.hilinecolor)
            Else
                Me.Plot4.Set(0, Me.m_prevtrlowest.Value, Me.lolinecolor)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_rs_dailydataarray1 = New [Function].RS_DailyDataArray(Me)
            Me.m_rs_trueextremes1 = New [Function].RS_TrueExtremes(Me)
            Me.m_rs_priceextension1 = New [Function].RS_PriceExtension(Me)
            Me.m_rs_reversalpatterns1 = New [Function].RS_ReversalPatterns(Me)
            Me.m_barnumber1 = New [Function].BarNumber(Me)
            Me.m_value3 = New VariableObject(Of Integer)(Me)
            Me.m_index = New VariableObject(Of Integer)(Me)
            Me.m_prevtrhighest = New VariableObject(Of Double)(Me)
            Me.m_prevtrlowest = New VariableObject(Of Double)(Me)
            Me.m_prevatr = New VariableObject(Of Double)(Me)
            Me.m_osetup = New VariableObject(Of Integer)(Me)
            Me.m_otrigger = New VariableObject(Of Double)(Me)
            Me.m_dataarray = New Array2DSimple(Of Double)(Me, 13, &H65)
            Me.m_subarray = New ArraySimple(Of Double)(Me, 4)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Ext", EPlotShapes.Point, Color.Blue, Color.Empty, 3, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("Setup", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("Trigger", EPlotShapes.Point, Color.Cyan, Color.Empty, 5, 0, True))
            Me.Plot4 = MyBase.AddPlot(New PlotAttributes("HiLoLines", EPlotShapes.Point, Color.Yellow, Color.Empty, 1, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_rs_dailydataarray1.numdays = Me.numdays
            Me.m_rs_dailydataarray1.odataarray = Me.m_dataarray
            Me.m_rs_dailydataarray1.oindex = Me.m_index
            Me.m_rs_dailydataarray1.osubarray = Me.m_subarray
            Me.m_rs_trueextremes1.numdays = Me.numdays
            Me.m_rs_trueextremes1.dataarray = Me.m_dataarray
            Me.m_rs_trueextremes1.index = Me.m_index
            Me.m_rs_trueextremes1.oprevtrhighest = Me.m_prevtrhighest
            Me.m_rs_trueextremes1.oprevtrlowest = Me.m_prevtrlowest
            Me.m_rs_trueextremes1.oprevatr = Me.m_prevatr
            Me.m_rs_priceextension1.numdays = Me.numdays
            Me.m_rs_priceextension1.minconsolindex = Me.minconsolindex
            Me.m_rs_priceextension1.finalrangefactor = Me.finalrangefactor
            Me.m_rs_priceextension1.prevtrhighest = Me.m_prevtrhighest
            Me.m_rs_priceextension1.prevtrlowest = Me.m_prevtrlowest
            Me.m_rs_priceextension1.prevatr = Me.m_prevatr
            Me.m_rs_priceextension1.dataarray = Me.m_dataarray
            Me.m_rs_priceextension1.index = Me.m_index
            Me.m_rs_reversalpatterns1.revdirection = Me.m_value3
            Me.m_rs_reversalpatterns1.revcriteria = Me.revcriteria
            Me.m_rs_reversalpatterns1.gapsizefactor = Me.gapsizefactor
            Me.m_rs_reversalpatterns1.dataarray = Me.m_dataarray
            Me.m_rs_reversalpatterns1.index = Me.m_index
            Me.m_rs_reversalpatterns1.subarray = Me.m_subarray
            Me.m_rs_reversalpatterns1.osetup = Me.m_osetup
            Me.m_rs_reversalpatterns1.otrigger = Me.m_otrigger
        End Sub


        ' Properties
        <Input> _
        Public Property finalrangefactor As Double

        <Input()> _
        Public Property gapsizefactor As Double

        <Input()> _
        Public Property hilinecolor As Color

        <Input()> _
        Public Property lolinecolor As Color

        <Input()> _
        Public Property minconsolindex As Double

        <Input()> _
        Public Property numdays As Integer

        <Input()> _
        Public Property revcriteria As Integer


        ' Fields
        Private m_barnumber1 As [Function].BarNumber
        Private m_dataarray As Array2DSimple(Of Double)
        Private m_index As VariableObject(Of Integer)
        Private m_osetup As VariableObject(Of Integer)
        Private m_otrigger As VariableObject(Of Double)
        Private m_prevatr As VariableObject(Of Double)
        Private m_prevtrhighest As VariableObject(Of Double)
        Private m_prevtrlowest As VariableObject(Of Double)
        Private m_rs_dailydataarray1 As [Function].RS_DailyDataArray
        Private m_rs_priceextension1 As [Function].RS_PriceExtension
        Private m_rs_reversalpatterns1 As [Function].RS_ReversalPatterns
        Private m_rs_trueextremes1 As [Function].RS_TrueExtremes
        Private m_subarray As ArraySimple(Of Double)
        Private m_value3 As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
        Private Plot4 As IPlotObject
    End Class
End Namespace
