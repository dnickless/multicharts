Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Brkout_of_X_Bar_High
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            m_highest.Value = Bars.High.Highest(Me.length)
            If PublicFunctions.DoubleGreater(MyBase.Bars.High.Item(0), m_highest.Item(1)) Then
                Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                MyBase.Alerts.Alert()
            End If
        End Sub

        Protected Overrides Sub Create()
            m_highest = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("BrkoutHi", EPlotShapes.Point, Color.Yellow, Color.Empty, 4, 0, True))
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private Plot1 As IPlotObject
        Private m_highest As VariableSeries(Of Double)
    End Class
End Namespace
