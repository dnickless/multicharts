Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_BullHar_BearHar
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_bullhar_bearhar1.Call()
            If (Me.m_obullishharami.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("BullishHarami", New Object(0 - 1) {})
            ElseIf (Me.m_obearishharami.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("BearishHarami", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_bullhar_bearhar1 = New [Function].C_BullHar_BearHar(Me)
            Me.m_obullishharami = New VariableObject(Of Integer)(Me)
            Me.m_obearishharami = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BullHar", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("BearHar", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_bullhar_bearhar1.length = Me.length
            Me.m_c_bullhar_bearhar1.obullishharami = Me.m_obullishharami
            Me.m_c_bullhar_bearhar1.obearishharami = Me.m_obearishharami
            Me.m_obullishharami.DefaultValue = 0
            Me.m_obearishharami.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_c_bullhar_bearhar1 As [Function].C_BullHar_BearHar
        Private m_obearishharami As VariableObject(Of Integer)
        Private m_obullishharami As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
