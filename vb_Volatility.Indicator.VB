Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volatility
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 14
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_volatility1.Value)
        End Sub

        Protected Overrides Sub Create()
            Me.m_volatility1 = New [Function].Volatility(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("Volty", EPlotShapes.Line, Color.Blue, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_volatility1.length = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_volatility1 As [Function].Volatility
        Private Plot1 As IPlotObject
    End Class
End Namespace
