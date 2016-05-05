Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_C_BullEng_BearEng
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.m_length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_c_bulleng_beareng1.Call
            If (Me.m_obullishengulfing.Value = 1) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert("BullishEngulfing", New Object(0 - 1) {})
            ElseIf (Me.m_obearishengulfing.Value = 1) Then
                Me.Plot2.Set(0, MyBase.Bars.Low.Item(0))
                MyBase.Alerts.Alert("BearishEngulfing", New Object(0 - 1) {})
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_c_bulleng_beareng1 = New [Function].C_BullEng_BearEng(Me)
            Me.m_obullishengulfing = New VariableObject(Of Integer)(Me)
            Me.m_obearishengulfing = New VariableObject(Of Integer)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BullEng", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("BearEng", EPlotShapes.Point, Color.Magenta, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_c_bulleng_beareng1.Length = Me.length
            Me.m_c_bulleng_beareng1.obullishengulfing = Me.m_obullishengulfing
            Me.m_c_bulleng_beareng1.obearishengulfing = Me.m_obearishengulfing
            Me.m_obullishengulfing.DefaultValue = 0
            Me.m_obearishengulfing.DefaultValue = 0
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer
            Get
                Return Me.m_length
            End Get
            Set(ByVal value As Integer)
                Me.m_length = value
            End Set
        End Property


        ' Fields
        Private m_c_bulleng_beareng1 As [Function].C_BullEng_BearEng
        Private m_length As Integer
        Private m_obearishengulfing As VariableObject(Of Integer)
        Private m_obullishengulfing As VariableObject(Of Integer)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
    End Class
End Namespace
