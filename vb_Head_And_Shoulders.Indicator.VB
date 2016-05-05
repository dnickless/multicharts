Imports PowerLanguage
Imports PowerLanguage.Function
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)> _
    Public Class vb_Head_And_Shoulders
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.color2 = Color.Cyan
            Me.color1 = Color.Yellow
            Me.barspast = 10
            Me.drawneckline = True
            Me.confirmvolume = True
            Me.hilo = 1
            Me.vertproportionlimit = 0.33
            Me.hnsswstrength = 2
            Me.neckswstrength = 2
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_barnum.Value = CDbl(Me.m_barnumber1.Item(0))
            If (Me.hilo = 1) Then
                Me.m_neckswbar.Value = Me.m_swinglowbar1.Item(0)
                Me.m_hnsswbar.Value = Me.m_swinghighbar1.Item(0)
                Me.m_neckswprice.Value = MyBase.Bars.Low.Item(0)
                Me.m_hnsswprice.Value = MyBase.Bars.High.Item(0)
            ElseIf (Me.hilo = -1) Then
                Me.m_neckswbar.Value = Me.m_swinghighbar2.Item(0)
                Me.m_hnsswbar.Value = Me.m_swinglowbar2.Item(0)
                Me.m_neckswprice.Value = MyBase.Bars.High.Item(0)
                Me.m_hnsswprice.Value = MyBase.Bars.Low.Item(0)
            End If
            If (Me.m_neckswbar.Value = Me.neckswstrength) Then
                Me.m_neckswings.Item(0)(1, 0) = Me.m_neckswings.Item(0)(0, 0)
                Me.m_neckswings.Item(0)(1, 1) = Me.m_neckswings.Item(0)(0, 1)
                Me.m_neckswings.Item(0)(1, 2) = Me.m_neckswings.Item(0)(0, 2)
                Me.m_neckswings.Item(0)(1, 3) = Me.m_neckswings.Item(0)(0, 3)
                Me.m_neckswings.Item(0)(0, 0) = Me.m_barnum.Item(Me.neckswstrength)
                Me.m_neckswings.Item(0)(0, 1) = Bars.Time.Item(Me.neckswstrength).ToOADate
                Me.m_neckswings.Item(0)(0, 2) = Bars.Time.Item(Me.neckswstrength).ToOADate
                Me.m_neckswings.Item(0)(0, 3) = Me.m_neckswprice.Item(Me.neckswstrength)
            End If
            If (Me.m_hnsswbar.Value = Me.hnsswstrength) Then
                Dim m_value1 As Integer = 1
                Do While PublicFunctions.DoubleGreaterEquals(CDbl(m_value1), 0)
                    Me.m_hnsswings.Item(0)((m_value1 + 1), 0) = Me.m_hnsswings.Item(0)(m_value1, 0)
                    Me.m_hnsswings.Item(0)((m_value1 + 1), 1) = Me.m_hnsswings.Item(0)(m_value1, 1)
                    Me.m_hnsswings.Item(0)((m_value1 + 1), 2) = Me.m_hnsswings.Item(0)(m_value1, 2)
                    Me.m_hnsswings.Item(0)((m_value1 + 1), 3) = Me.m_hnsswings.Item(0)(m_value1, 3)
                    Me.m_hnsswings.Item(0)((m_value1 + 1), 4) = Me.m_hnsswings.Item(0)(m_value1, 4)
                    m_value1 -= 1
                Loop
                Me.m_hnsswings.Item(0)(0, 0) = Me.m_barnum.Item(Me.hnsswstrength)
                Me.m_hnsswings.Item(0)(0, 1) = Bars.Time.Item(Me.hnsswstrength).ToOADate
                Me.m_hnsswings.Item(0)(0, 2) = MyBase.Bars.Time.Item(Me.hnsswstrength).ToOADate
                Me.m_hnsswings.Item(0)(0, 3) = Me.m_hnsswprice.Item(Me.hnsswstrength)
                Me.m_hnsswings.Item(0)(0, 4) = PublicFunctions.Average(MyBase.Bars.TrueVolume(), (Me.hnsswstrength * 2))
                Dim m_hororder As Boolean = ((((Me.m_hnsswings.Item(0)(0, 0) > Me.m_neckswings.Item(0)(0, 0)) AndAlso (Me.m_neckswings.Item(0)(0, 0) > Me.m_hnsswings.Item(0)(1, 0))) AndAlso (Me.m_hnsswings.Item(0)(1, 0) > Me.m_neckswings.Item(0)(1, 0))) AndAlso (Me.m_neckswings.Item(0)(1, 0) > Me.m_hnsswings.Item(0)(2, 0)))
                If (Me.hilo = 1) Then
                    Me.m_vertorder_hvss.Value = ((Me.m_hnsswings.Item(0)(1, 3) > Me.m_hnsswings.Item(0)(2, 3)) AndAlso (Me.m_hnsswings.Item(0)(1, 3) > Me.m_hnsswings.Item(0)(0, 3)))
                    Me.m_vertorder_nvss.Value = ((Me.m_neckswings.Item(0)(0, 3) < Me.m_hnsswings.Item(0)(2, 3)) AndAlso (Me.m_neckswings.Item(0)(1, 3) < Me.m_hnsswings.Item(0)(0, 3)))
                    Me.m_necklinelimit.Value = PublicFunctions.Min(Me.m_neckswings.Item(0)(0, 3), New Double() {Me.m_neckswings.Item(0)(1, 3)})
                    Me.m_maxnecktoheaddist.Value = (Me.m_hnsswings.Item(0)(1, 3) - Me.m_necklinelimit.Value)
                ElseIf (Me.hilo = -1) Then
                    Me.m_vertorder_hvss.Value = ((Me.m_hnsswings.Item(0)(1, 3) < Me.m_hnsswings.Item(0)(2, 3)) AndAlso (Me.m_hnsswings.Item(0)(1, 3) < Me.m_hnsswings.Item(0)(0, 3)))
                    Me.m_vertorder_nvss.Value = ((Me.m_neckswings.Item(0)(0, 3) > Me.m_hnsswings.Item(0)(2, 3)) AndAlso (Me.m_neckswings.Item(0)(1, 3) > Me.m_hnsswings.Item(0)(0, 3)))
                    Me.m_necklinelimit.Value = PublicFunctions.Max(Me.m_neckswings.Item(0)(0, 3), New Double() {Me.m_neckswings.Item(0)(1, 3)})
                    Me.m_maxnecktoheaddist.Value = (Me.m_necklinelimit.Value - Me.m_hnsswings.Item(0)(1, 3))
                End If
                Dim m_necklinetilt As Double = Math.Abs(CDbl((Me.m_neckswings.Item(0)(0, 3) - Me.m_neckswings.Item(0)(1, 3))))
                If (Not Me.m_maxnecktoheaddist.Value = 0) Then
                    Me.m_vertproportion.Value = PublicFunctions.DoubleLess((m_necklinetilt / Me.m_maxnecktoheaddist.Value), Me.vertproportionlimit)
                Else
                    Me.m_vertproportion.Value = False
                End If
                If Not If(((Not m_hororder OrElse Not Me.m_vertorder_hvss.Value) OrElse Not Me.m_vertorder_nvss.Value), True, Not Me.m_vertproportion.Value) Then
                    If (Me.m_color.Value = Me.color2) Then
                        Me.m_color.Value = Me.color1
                    Else
                        Me.m_color.Value = Me.color2
                    End If
                    Dim m_textref As ITextObject = MyBase.DrwText.Create(New ChartPoint(DateTime.FromOADate(Me.m_hnsswings.Item(0)(0, 2)), Me.m_hnsswings.Item(0)(0, 3)), "Shoulder")
                    m_textref.Color = Me.m_color.Value
                    m_textref.HStyle = ETextStyleH.Center
                    m_textref.VStyle = ETextStyleV.Below
                    m_textref = MyBase.DrwText.Create(New ChartPoint(DateTime.FromOADate(Me.m_hnsswings.Item(0)(1, 2)), Me.m_hnsswings.Item(0)(1, 3)), "Head")
                    m_textref.Color = Me.m_color.Value
                    m_textref.HStyle = ETextStyleH.Center
                    m_textref.VStyle = ETextStyleV.Below
                    m_textref = MyBase.DrwText.Create(New ChartPoint(DateTime.FromOADate(Me.m_hnsswings.Item(0)(2, 2)), Me.m_hnsswings.Item(0)(2, 3)), "Shoulder")
                    m_textref.Color = Me.m_color.Value
                    m_textref.HStyle = ETextStyleH.Center
                    m_textref.VStyle = ETextStyleV.Below
                    If Me.confirmvolume Then
                        Dim m_msg As String
                        Dim m_avgvolfast As Double = PublicFunctions.Average(MyBase.Bars.TrueVolume(), Me.hnsswstrength)
                        If (PublicFunctions.DoubleGreater(m_avgvolfast, Me.m_hnsswings.Item(0)(0, 4)) AndAlso PublicFunctions.DoubleGreater(m_avgvolfast, Me.m_hnsswings.Item(0)(2, 4))) Then
                            m_msg = "Volume confirm"
                        Else
                            m_msg = "Vol non-confirm"
                        End If
                        m_textref = MyBase.DrwText.Create(New ChartPoint(DateTime.FromOADate(Me.m_neckswings.Item(0)(1, 2)), Me.m_necklinelimit.Value), m_msg)
                        m_textref.Color = Me.m_color.Value
                        m_textref.HStyle = ETextStyleH.Right
                        m_textref.VStyle = ETextStyleV.Above
                    End If
                    If Me.drawneckline Then
                        Me.m_tlref = MyBase.DrwTrendLine.Create(New ChartPoint(DateTime.FromOADate(Me.m_neckswings.Item(0)(1, 2)), Me.m_neckswings.Item(0)(1, 3)), New ChartPoint(DateTime.FromOADate(Me.m_neckswings.Item(0)(0, 2)), Me.m_neckswings.Item(0)(0, 3)))
                        Me.m_tlref.ExtLeft = False
                        Me.m_tlref.ExtRight = False
                        Me.m_tlref.Color = Me.m_color.Value
                        Me.m_confirmbarnum.Value = Me.m_barnum.Value
                    End If
                End If
            End If
            If ((((Not Me.m_tlref Is Nothing) AndAlso Me.m_tlref.Exist) AndAlso (Not Me.m_lastcrossedtl.Value = Me.m_tlref.ID)) AndAlso PublicFunctions.DoubleLessEquals((Me.m_barnum.Value - Me.m_confirmbarnum.Value), CDbl(Me.barspast))) Then
                Dim m_tl_value As Double = Me.m_tlref.PriceValue(MyBase.Bars.Time.Item(0))
                If (((Me.hilo = 1) AndAlso PublicFunctions.DoubleLess(MyBase.Bars.Close.Item(0), m_tl_value)) OrElse ((Me.hilo = -1) AndAlso PublicFunctions.DoubleGreater(MyBase.Bars.Close.Item(0), m_tl_value))) Then
                    Me.Plot1.Set(0, Me.m_neckswprice.Value, Me.m_color.Value)
                    MyBase.Alerts.Alert()
                    Me.m_lastcrossedtl.Value = Me.m_tlref.ID
                Else
                    Me.Plot1.Reset()
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_barnumber1 = New BarNumber(Me)
            Me.m_swinglowbar1 = New SwingLowBar(Me)
            Me.m_swinglowbar2 = New SwingLowBar(Me)
            Me.m_swinghighbar1 = New SwingHighBar(Me)
            Me.m_swinghighbar2 = New SwingHighBar(Me)
            Me.m_barnum = New VariableSeries(Of Double)(Me)
            Me.m_neckswbar = New VariableObject(Of Integer)(Me)
            Me.m_hnsswbar = New VariableObject(Of Integer)(Me)
            Me.m_neckswprice = New VariableSeries(Of Double)(Me)
            Me.m_hnsswprice = New VariableSeries(Of Double)(Me)
            Me.m_vertorder_hvss = New VariableObject(Of Boolean)(Me)
            Me.m_vertorder_nvss = New VariableObject(Of Boolean)(Me)
            Me.m_necklinelimit = New VariableObject(Of Double)(Me)
            Me.m_maxnecktoheaddist = New VariableObject(Of Double)(Me)
            Me.m_vertproportion = New VariableObject(Of Boolean)(Me)
            Me.m_color = New VariableObject(Of Color)(Me)
            Me.m_confirmbarnum = New VariableObject(Of Double)(Me)
            Me.m_lastcrossedtl = New VariableObject(Of Integer)(Me)
            Me.m_neckswings = New Array2DSimple(Of Double)(Me, 2, 4)
            Me.m_hnsswings = New Array2DSimple(Of Double)(Me, 3, 5)
            Me.Plot1 = MyBase.AddPlot(New PlotAttributes("H&S", EPlotShapes.Point, Color.White, Color.Empty, 4, 0, True))
        End Sub

        Protected Overrides Sub StartCalc()
            Dim mNeckswlength As Integer = (Me.neckswstrength + 1)
            Dim mHnsswlength As Integer = (Me.hnsswstrength + 1)
            Me.m_swinglowbar1.instance = 1
            Me.m_swinglowbar1.price = MyBase.Bars.Low
            Me.m_swinglowbar1.strength = Me.neckswstrength
            Me.m_swinglowbar1.length = mNeckswlength
            Me.m_swinglowbar2.instance = 1
            Me.m_swinglowbar2.price = MyBase.Bars.Low
            Me.m_swinglowbar2.strength = Me.hnsswstrength
            Me.m_swinglowbar2.length = mHnsswlength
            Me.m_swinghighbar1.instance = 1
            Me.m_swinghighbar1.price = MyBase.Bars.High
            Me.m_swinghighbar1.strength = Me.hnsswstrength
            Me.m_swinghighbar1.length = mHnsswlength
            Me.m_swinghighbar2.instance = 1
            Me.m_swinghighbar2.price = MyBase.Bars.High
            Me.m_swinghighbar2.strength = Me.neckswstrength
            Me.m_swinghighbar2.length = mNeckswlength
            Me.m_color.DefaultValue = Me.color2
            Me.m_tlref = Nothing
        End Sub


        ' Properties
        <Input> _
        Public Property barspast As Integer

        <Input()> _
        Public Property color1 As Color

        <Input()> _
        Public Property color2 As Color

        <Input()> _
        Public Property confirmvolume As Boolean

        <Input()> _
        Public Property drawneckline As Boolean

        <Input()> _
        Public Property hilo As Integer

        <Input()> _
        Public Property hnsswstrength As Integer

        <Input()> _
        Public Property neckswstrength As Integer

        <Input()> _
        Public Property vertproportionlimit As Double


        ' Fields
        Private m_barnum As VariableSeries(Of Double)
        Private m_barnumber1 As BarNumber
        Private m_color As VariableObject(Of Color)
        Private m_confirmbarnum As VariableObject(Of Double)
        Private m_hnsswbar As VariableObject(Of Integer)
        Private m_hnsswings As Array2DSimple(Of Double)
        Private m_hnsswprice As VariableSeries(Of Double)
        Private m_lastcrossedtl As VariableObject(Of Integer)
        Private m_maxnecktoheaddist As VariableObject(Of Double)
        Private m_necklinelimit As VariableObject(Of Double)
        Private m_neckswbar As VariableObject(Of Integer)
        Private m_neckswings As Array2DSimple(Of Double)
        Private m_neckswprice As VariableSeries(Of Double)
        Private m_swinghighbar1 As SwingHighBar
        Private m_swinghighbar2 As SwingHighBar
        Private m_swinglowbar1 As SwingLowBar
        Private m_swinglowbar2 As SwingLowBar
        Private m_tlref As ITrendLineObject
        Private m_vertorder_hvss As VariableObject(Of Boolean)
        Private m_vertorder_nvss As VariableObject(Of Boolean)
        Private m_vertproportion As VariableObject(Of Boolean)
        Private Plot1 As IPlotObject
    End Class
End Namespace
