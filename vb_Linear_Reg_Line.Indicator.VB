Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Linear_Reg_Line
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.extright = True
            Me.color = Color.Yellow
            Me.endtime = DateTime.MinValue
            Me.length = 30
        End Sub

        Protected Overrides Sub CalcBar()
            Dim m_lrv As Double
            Dim m_lrvago As Double
            If (Me.m_flag.Value = 0) Then
                If ((Me.endtime = DateTime.MinValue) AndAlso MyBase.Bars.LastBarOnChart) Then
                    m_lrv = Me.m_linearregvalue1.Item(0)
                    m_lrvago = Me.m_linearregvalue2.Item(0)
                    Me.m_tllrv.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item((Me.length - 1)), m_lrvago), New ChartPoint(MyBase.Bars.Time.Item(0), m_lrv))
                    Me.m_flag.Value = 1
                ElseIf (MyBase.Bars.Time.Item(0) = Me.endtime) Then
                    m_lrv = Me.m_linearregvalue3.Item(0)
                    m_lrvago = Me.m_linearregvalue4.Item(0)
                    Me.m_tllrv.Value = MyBase.DrwTrendLine.Create(New ChartPoint(MyBase.Bars.Time.Item((Me.length - 1)), m_lrvago), New ChartPoint(MyBase.Bars.Time.Item(0), m_lrv))
                    Me.m_flag.Value = 2
                End If
                If ((Me.m_flag.Value = 1) OrElse (Me.m_flag.Value = 2)) Then
                    Me.m_tllrv.Value.Color = Me.color
                    Me.m_tllrv.Value.ExtLeft = False
                    If Me.extright Then
                        Me.m_tllrv.Value.ExtRight = True
                    Else
                        Me.m_tllrv.Value.ExtRight = False
                    End If
                End If
            ElseIf (Me.m_flag.Value = 1) Then
                m_lrv = Me.m_linearregvalue5.Item(0)
                m_lrvago = Me.m_linearregvalue6.Item(0)
                Me.m_tllrv.Value.Begin = New ChartPoint(MyBase.Bars.Time.Item((Me.length - 1)), m_lrvago)
                Me.m_tllrv.Value.End = New ChartPoint(MyBase.Bars.Time.Item(0), m_lrv)
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_linearregvalue1 = New LinearRegValue(Me)
            Me.m_linearregvalue2 = New LinearRegValue(Me)
            Me.m_linearregvalue3 = New LinearRegValue(Me)
            Me.m_linearregvalue4 = New LinearRegValue(Me)
            Me.m_linearregvalue5 = New LinearRegValue(Me)
            Me.m_linearregvalue6 = New LinearRegValue(Me)
            Me.m_tllrv = New VariableObject(Of ITrendLineObject)(Me)
            Me.m_flag = New VariableObject(Of Integer)(Me)
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_linearregvalue1.price = MyBase.Bars.Close
            Me.m_linearregvalue1.length = Me.length
            Me.m_linearregvalue1.tgtbar = 0
            Me.m_linearregvalue2.price = MyBase.Bars.Close
            Me.m_linearregvalue2.length = Me.length
            Me.m_linearregvalue2.tgtbar = (Me.length - 1)
            Me.m_linearregvalue3.price = MyBase.Bars.Close
            Me.m_linearregvalue3.length = Me.length
            Me.m_linearregvalue3.tgtbar = 0
            Me.m_linearregvalue4.price = MyBase.Bars.Close
            Me.m_linearregvalue4.length = Me.length
            Me.m_linearregvalue4.tgtbar = (Me.length - 1)
            Me.m_linearregvalue5.price = MyBase.Bars.Close
            Me.m_linearregvalue5.length = Me.length
            Me.m_linearregvalue5.tgtbar = 0
            Me.m_linearregvalue6.price = MyBase.Bars.Close
            Me.m_linearregvalue6.length = Me.length
            Me.m_linearregvalue6.tgtbar = Me.length
            Me.m_tllrv.DefaultValue = Nothing
            Me.m_flag.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property color As Color

        <Input()> _
        Public Property endtime As DateTime

        <Input()> _
        Public Property extright As Boolean

        <Input()> _
        Public Property length As Integer


        ' Fields
        Private m_flag As VariableObject(Of Integer)
        Private m_linearregvalue1 As LinearRegValue
        Private m_linearregvalue2 As LinearRegValue
        Private m_linearregvalue3 As LinearRegValue
        Private m_linearregvalue4 As LinearRegValue
        Private m_linearregvalue5 As LinearRegValue
        Private m_linearregvalue6 As LinearRegValue
        Private m_tllrv As VariableObject(Of ITrendLineObject)
    End Class
End Namespace
