Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Standard_Error_Bands
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.smoothinglength = 3
            Me.linreglength = &H15
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_linregvalue.Value = Me.m_linearregvalue1.Item(0)
            Dim m_linregavg As Double = PublicFunctions.Average(Me.m_linregvalue, Me.smoothinglength)
            Dim m_serroravg As Double = PublicFunctions.Average(Me.m_stderror1, Me.smoothinglength)
            Me.m_lowerband.Value = (m_linregavg - m_serroravg)
            Me.m_upperband.Value = (m_linregavg + m_serroravg)
            If (((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) AndAlso (MyBase.Bars.CurrentBar >= Me.smoothinglength)) Then
                Me.Plot1.Set(Me.displace, Me.m_upperband.Value)
                Me.Plot2.Set(Me.displace, Me.m_lowerband.Value)
                Me.Plot3.Set(Me.displace, m_linregavg)
                If (Me.displace <= 0) Then
                    If Me.CrossesOver(Me.price, Me.m_lowerband) Then
                        MyBase.Alerts.Alert("Price crossing over lower price band", New Object(0  - 1) {})
                    ElseIf Me.CrossesUnder(Me.price, Me.m_upperband) Then
                        MyBase.Alerts.Alert("Price crossing under upper price band", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_linearregvalue1 = New LinearRegValue(Me)
            Me.m_stderror1 = New StdError(Me)
            Me.m_linregvalue = New VariableSeries(Of Double)(Me)
            Me.m_lowerband = New VariableSeries(Of Double)(Me)
            Me.m_upperband = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UpperBand", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
            Me.Plot2 = MyBase.AddPlot(New PlotAttributes("LowerBand", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
            Me.Plot3 = MyBase.AddPlot(New PlotAttributes("MidLine", EPlotShapes.Line, Color.Gray, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.price = MyBase.Bars.Close
            Me.m_linearregvalue1.price = Me.price
            Me.m_linearregvalue1.length = Me.linreglength
            Me.m_linearregvalue1.tgtbar = 0
            Me.m_stderror1.price = Me.price
            Me.m_stderror1.length = Me.linreglength
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property linreglength As Integer

        Private Property price As ISeries(Of Double)

        <Input()> _
        Public Property smoothinglength As Integer


        ' Fields
        Private m_linearregvalue1 As LinearRegValue
        Private m_linregvalue As VariableSeries(Of Double)
        Private m_lowerband As VariableSeries(Of Double)
        Private m_stderror1 As StdError
        Private m_upperband As VariableSeries(Of Double)
        Private Plot1 As IPlotObject
        Private Plot2 As IPlotObject
        Private Plot3 As IPlotObject
    End Class
End Namespace
