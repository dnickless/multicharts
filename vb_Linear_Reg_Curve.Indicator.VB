Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Linear_Reg_Curve
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.length = 9
        End Sub

        Protected Overrides Sub CalcBar()
            If ((Me.displace >= 0) OrElse (MyBase.Bars.CurrentBar > Math.Abs(Me.displace))) Then
                Me.m_linreg.Value = Me.m_linearregvalue1.Item(0)
                Me.Plot1.Set(Me.displace, Me.m_linreg.Value)
                If (Me.displace <= 0) Then
                    If ((PublicFunctions.DoubleGreater(Me.price.Item(0), Me.m_linreg.Value) AndAlso PublicFunctions.DoubleGreater(Me.m_linreg.Value, Me.m_linreg.Item(1))) AndAlso PublicFunctions.DoubleLessEquals(Me.m_linreg.Item(1), Me.m_linreg.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning up", New Object(0  - 1) {})
                    ElseIf ((PublicFunctions.DoubleLess(Me.price.Item(0), Me.m_linreg.Value) AndAlso PublicFunctions.DoubleLess(Me.m_linreg.Value, Me.m_linreg.Item(1))) AndAlso PublicFunctions.DoubleGreaterEquals(Me.m_linreg.Item(1), Me.m_linreg.Item(2))) Then
                        MyBase.Alerts.Alert("Indicator turning down", New Object(0  - 1) {})
                    End If
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_linearregvalue1 = New [Function].LinearRegValue(Me)
            Me.m_linreg = New VariableSeries(Of Double)(Me)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("LinReg", EPlotShapes.Line, Color.Yellow, Color.Empty, 0, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_price = MyBase.Bars.Close
            Me.m_linearregvalue1.price = Me.price
            Me.m_linearregvalue1.length = Me.length
            Me.m_linearregvalue1.tgtbar = 0
        End Sub


        ' Properties
        <Input> _
        Public Property displace As Integer

        <Input()> _
        Public Property length As Integer

        Private ReadOnly Property price As ISeries(Of Double)
            Get
                Return Me.m_price
            End Get
        End Property


        ' Fields
        Private m_linearregvalue1 As [Function].LinearRegValue
        Private m_linreg As VariableSeries(Of Double)
        Private m_price As ISeries(Of Double)
        Private Plot1 As IPlotObject
    End Class
End Namespace
