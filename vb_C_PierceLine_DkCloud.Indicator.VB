Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_PierceLine_DkCloud
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_pierceline_dkcloud1.Call
            If (Me.m_opiercingline.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("PiercingLine", New Object(0 - 1) {})
            ElseIf (Me.m_odarkcloud.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("DarkCloud", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_pierceline_dkcloud1 = New [Function].C_PierceLine_DkCloud(Me)
            Me.m_opiercingline = New VariableObject(Of Integer)(Me)
            Me.m_odarkcloud = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("PierceLine", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("DkCloud", EPlotShapes.Point, Color.Magenta, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_pierceline_dkcloud1.length = Me.length
            Me.m_c_pierceline_dkcloud1.opiercingline = Me.m_opiercingline
            Me.m_c_pierceline_dkcloud1.odarkcloud = Me.m_odarkcloud
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_c_pierceline_dkcloud1 As [Function].C_PierceLine_DkCloud
        Private m_odarkcloud As VariableObject(Of Integer)
        Private m_opiercingline As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
