Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Volume_Brkout
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.brkoutpct = 50
            Me.avglength = 50
        End Sub

        Protected Overrides Sub CalcBar()
            If PublicFunctions.DoubleGreaterEquals(Me.Volume.Item(0), (PublicFunctions.Average(Me.Volume, Me.avglength) * Me.m_brkoutfactor)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VolBrkOut", EPlotShapes.Point, Color.Cyan, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_brkoutfactor = (1 + (Me.brkoutpct * 0.01))
            Me.Volume = MyBase.Bars.TrueVolume()
        End Sub


        ' Properties
        <Input> _
        Public Property avglength As Integer

        <Input()> _
        Public Property brkoutpct As Double

        Private Property Volume As ISeries(Of Double)


        ' Fields
        Private m_brkoutfactor As Double
        Private Plot1 As IPlotObject
    End Class
End Namespace
