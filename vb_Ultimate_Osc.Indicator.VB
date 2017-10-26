Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    Public Class vb_Ultimate_Osc
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.slowlength = &H1C
            Me.medlength = 14
            Me.fastlength = 7
        End Sub

        Protected Overrides Sub CalcBar()
            Me.Plot1.Set(0, Me.m_ultimateoscillator1.Item(0))
        End Sub

        Protected Overrides Sub Create()
            Me.m_ultimateoscillator1 = New UltimateOscillator(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("UltOsc", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_ultimateoscillator1.shortlength = Me.fastlength
            Me.m_ultimateoscillator1.middllength = Me.medlength
            Me.m_ultimateoscillator1.longlength = Me.slowlength
        End Sub


        ' Properties
        <Input> _
        Public Property fastlength As Integer

        <Input()> _
        Public Property medlength As Integer

        <Input()> _
        Public Property slowlength As Integer


        ' Fields
        Private m_ultimateoscillator1 As UltimateOscillator
        Private Plot1 As IPlotObject
    End Class
End Namespace
