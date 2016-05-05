Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Pivot_High
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.rightstrength = 3
            Me.leftstrength = 3
        End Sub

        Protected Overrides Sub CalcBar()
            If (Not Me.m_pivothighvsbar1.Item(0) = -1) Then
                Me.Plot1.Set(Me.rightstrength, MyBase.Bars.High.Item(Me.rightstrength))
                MyBase.Alerts.Alert()
            Else
                Me.Plot1.Reset()
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_pivothighvsbar1 = New PivotHighVSBar(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("PivotHi", EPlotShapes.Point, Color.Blue, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_pivothighvsbar1.instance = 1
            Me.m_pivothighvsbar1.price = MyBase.Bars.High
            Me.m_pivothighvsbar1.leftstrength = Me.leftstrength
            Me.m_pivothighvsbar1.rightstrength = Me.rightstrength
            Me.m_pivothighvsbar1.length = (Me.rightstrength + 1)
        End Sub


        ' Properties
        <Input> _
        Public Property leftstrength As Integer

        <Input()> _
        Public Property rightstrength As Integer


        ' Fields
        Private m_pivothighvsbar1 As PivotHighVSBar
        Private Plot1 As IPlotObject
    End Class
End Namespace
