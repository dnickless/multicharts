Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Day_OpenHiLo_Lines
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.datanum = 1
            Me.lowcolor = Color.Cyan
            Me.highcolor = Color.Yellow
            Me.opencolor = Color.Red
        End Sub

        Protected Overrides Sub CalcBar()
            Dim resolution As EResolution = MyBase.Bars.Info.Resolution.Type
            If ((((resolution < EResolution.Day) OrElse (EResolution.Volume = resolution)) OrElse (EResolution.Second = resolution)) OrElse (resolution > EResolution.Quarter)) Then
                If ((MyBase.Bars.TimeValue.Date <> Bars.Time.Item(1).Date) AndAlso (MyBase.Bars.Status = EBarState.Close)) Then
                    If Me.m_haveprevlines.Value Then
                        Me.m_tlopen.Value.End = New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_dayopen.Value)
                        Me.m_tlhigh.Value.End = New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_dayhigh.Value)
                        Me.m_tllow.Value.End = New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_daylow.Value)
                        Me.m_tlopen.Value.ExtRight = False
                        Me.m_tlhigh.Value.ExtRight = False
                        Me.m_tllow.Value.ExtRight = False
                    End If
                    Me.m_dayopen.Value = MyBase.Bars.Open.Item(0)
                    Me.m_dayhigh.Value = MyBase.Bars.High.Item(0)
                    Me.m_daylow.Value = MyBase.Bars.Low.Item(0)
                    Me.m_currbartime.Value = MyBase.Bars.Time.Item(0)
                    Me.m_lastdiffbartime.Value = MyBase.Bars.Time.Item(1)
                    Me.m_tlopen.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_dayopen.Value), New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_dayopen.Value))
                    Me.m_tlhigh.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_dayhigh.Value), New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_dayhigh.Value))
                    Me.m_tllow.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item(1), Me.m_daylow.Value), New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_daylow.Value))
                    Me.m_tlopen.Value.Color = Me.opencolor
                    Me.m_tlhigh.Value.Color = Me.highcolor
                    Me.m_tllow.Value.Color = Me.lowcolor
                    Me.m_tlopen.Value.ExtLeft = False
                    Me.m_tlhigh.Value.ExtLeft = False
                    Me.m_tllow.Value.ExtLeft = False
                    Me.m_tlopen.Value.ExtRight = True
                    Me.m_tlhigh.Value.ExtRight = True
                    Me.m_tllow.Value.ExtRight = True
                    If Not Me.m_haveprevlines.Value Then
                        Me.m_haveprevlines.Value = True
                    End If
                ElseIf Me.m_haveprevlines.Value Then
                    If (MyBase.Bars.Time.Item(0) <> Me.m_currbartime.Value) Then
                        Me.m_lastdiffbartime.Value = Me.m_currbartime.Value
                        Me.m_currbartime.Value = MyBase.Bars.Time.Item(0)
                    End If
                    If PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), Me.m_dayhigh.Value) Then
                        Me.m_dayhigh.Value = MyBase.Bars.High.Item(0)
                        Me.m_tlhigh.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_dayhigh.Value)
                        Me.m_tlhigh.Value.Begin = New ChartPoint(Me.m_lastdiffbartime.Value, Me.m_dayhigh.Value)
                    End If
                    If PublicFunctions.DoubleLess(MyBase.Bars.Low.Item(0), Me.m_daylow.Value) Then
                        Me.m_daylow.Value = MyBase.Bars.Low.Item(0)
                        Me.m_tllow.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), Me.m_daylow.Value)
                        Me.m_tllow.Value.Begin = New ChartPoint(Me.m_lastdiffbartime.Value, Me.m_daylow.Value)
                    End If
                End If
            Else
                PublicFunctions.RaiseRunTimeError("Day Open-Hi-Lo Lines requires intraday bars.")
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_haveprevlines = New VariableObject(Of Boolean)(Me)
            Me.m_tlopen = New VariableObject(Of ITrendLineObject)(Me)
            Me.m_tlhigh = New VariableObject(Of ITrendLineObject)(Me)
            Me.m_tllow = New VariableObject(Of ITrendLineObject)(Me)
            Me.m_dayopen = New VariableObject(Of Double)(Me)
            Me.m_dayhigh = New VariableObject(Of Double)(Me)
            Me.m_daylow = New VariableObject(Of Double)(Me)
            Me.m_currbartime = New VariableObject(Of DateTime)(Me)
            Me.m_lastdiffbartime = New VariableObject(Of DateTime)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_haveprevlines.DefaultValue = False
            Me.m_tlopen.DefaultValue = Nothing
            Me.m_tlhigh.DefaultValue = Nothing
            Me.m_tllow.DefaultValue = Nothing
            Me.m_dayopen.DefaultValue = 0
            Me.m_dayhigh.DefaultValue = 0
            Me.m_daylow.DefaultValue = 0
            Me.m_currbartime.DefaultValue = DateTime.MinValue
            Me.m_lastdiffbartime.DefaultValue = DateTime.MinValue
        End Sub


        ' Properties
        <Input> _
        Public Property datanum As Double

        <Input()> _
        Public Property highcolor As Color

        <Input()> _
        Public Property lowcolor As Color

        <Input()> _
        Public Property opencolor As Color


        ' Fields
        Private m_currbartime As VariableObject(Of DateTime)
        Private m_dayhigh As VariableObject(Of Double)
        Private m_daylow As VariableObject(Of Double)
        Private m_dayopen As VariableObject(Of Double)
        Private m_haveprevlines As VariableObject(Of Boolean)
        Private m_lastdiffbartime As VariableObject(Of DateTime)
        Private m_tlhigh As VariableObject(Of ITrendLineObject)
        Private m_tllow As VariableObject(Of ITrendLineObject)
        Private m_tlopen As VariableObject(Of ITrendLineObject)
    End Class
End Namespace
