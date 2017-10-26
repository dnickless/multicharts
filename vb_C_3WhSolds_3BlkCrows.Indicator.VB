Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_3WhSolds_3BlkCrows
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.percent = 5
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_3whsolds_3blkcrows1.Call
            If (Me.m_o3whitesoldiers.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("3WhiteSoldiers", New Object(0 - 1) {})
            ElseIf (Me.m_o3blackcrows.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("3BlackCrows", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_3whsolds_3blkcrows1 = New [Function].C_3WhSolds_3BlkCrows(Me)
            Me.m_o3whitesoldiers = New VariableObject(Of Integer)(Me)
            Me.m_o3blackcrows = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("3WhSolds", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("3BlkCrows", EPlotShapes.Point, Color.Magenta, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_3whsolds_3blkcrows1.length = Me.length
            Me.m_c_3whsolds_3blkcrows1.percent = Me.percent
            Me.m_c_3whsolds_3blkcrows1.o3whitesoldiers = Me.m_o3whitesoldiers
            Me.m_c_3whsolds_3blkcrows1.o3blackcrows = Me.m_o3blackcrows
            Me.m_o3whitesoldiers.DefaultValue = 0
            Me.m_o3blackcrows.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property percent As Double


        ' Fields
        Private m_c_3whsolds_3blkcrows1 As [Function].C_3WhSolds_3BlkCrows
        Private m_o3blackcrows As VariableObject(Of Integer)
        Private m_o3whitesoldiers As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
