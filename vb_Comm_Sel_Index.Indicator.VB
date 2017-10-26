Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Comm_Sel_Index
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
            Me.mycommission = 25
            Me.mymargin = 1000
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_csi1.Value)
        End Sub

        Protected Overrides Sub Create()
            Me.m_csi1 = New CSI(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("CSI", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_csi1.mymargin = Me.mymargin
            Me.m_csi1.mycommission = Me.mycommission
            Me.m_csi1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property mycommission As Double

        <Input()> _
        Public Property mymargin As Double


        ' Fields
        Private m_csi1 As CSI
        Private Plot1 As IPlotObject
    End Class
End Namespace
