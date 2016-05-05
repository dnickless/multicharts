Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Volatility_Std_Dev
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 30
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_volatilitystddev1.Item(0))
        End Sub

        Protected Overrides Sub Create()
            Me.m_volatilitystddev1 = New [Function].VolatilityStdDev(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("VoltySDev", EPlotShapes.Line, Color.Cyan, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_volatilitystddev1.numdays = Me.length
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer


        ' Fields
        Private m_volatilitystddev1 As [Function].VolatilityStdDev
        Private Plot1 As IPlotObject
    End Class
End Namespace
