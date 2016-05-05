Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Count_Criteria_SM
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.textcolor = Color.Cyan
            Me.occur = 7
            Me.length = 14
            Me.showtext = True
        End Sub

        Protected Overrides Sub CalcBar()
            If Me.m_criteria.Invoke(0) Then
                Dim m_count As Integer = Me.m_criteria.CountIF(Me.length)
                If PublicFunctions.DoubleGreaterEquals(CDbl(m_count), Me.occur) Then
                    Me.Plot1.Set(0, MyBase.Bars.High.Item(0))
                    MyBase.Alerts.Alert
                    If Me.showtext Then
                        Me.m_textid = MyBase.DrwText.Create(New ChartPoint(MyBase.Bars.Time.Item(0), MyBase.Bars.High.Item(0)), m_count.ToString)
                        Me.m_textid.HStyle = ETextStyleH.Center
                        Me.m_textid.VStyle = ETextStyleV.Above
                        Me.m_textid.Color = Me.textcolor
                    End If
                End If
            Else
                Me.Plot1.Reset
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("CntCrtSM", EPlotShapes.Point, Color.Cyan, Color.Empty, 3, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_criteria = Function(bb) PublicFunctions.DoubleGreater(Bars.Close(bb), Bars.High(1 + bb))
        End Sub


        ' Properties
        <Input> _
        Public Property length As Integer

        <Input()> _
        Public Property occur As Double

        <Input()> _
        Public Property showtext As Boolean

        <Input()> _
        Public Property textcolor As Color


        ' Fields
        Private m_criteria As Func(Of Integer, Boolean)
        Private m_textid As ITextObject
        Private Plot1 As IPlotObject
    End Class
End Namespace
