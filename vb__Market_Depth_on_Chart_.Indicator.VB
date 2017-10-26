Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <RecoverDrawings(False), SameAsSymbol(True)> _
    Public Class vb__Market_Depth_on_Chart_
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.updatespeed_seconds = 0.1
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Bars.LastBarOnChart Then
                MyBase.ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(Me.updatespeed_seconds))
                If MyBase.Bars.DOM.Connected Then
                    Me.m_value11 = 0
                    Dim domPrice As DOMPrice
                    For Each domPrice In MyBase.Bars.DOM.Ask
                        Me.m_value11 = Math.Max(Me.m_value11, domPrice.Size)
                    Next
                    Dim domPrice1 As DOMPrice
                    For Each domPrice1 In MyBase.Bars.DOM.Bid
                        Me.m_value11 = Math.Max(Me.m_value11, domPrice1.Size)
                    Next
                    Me.m_draw_dom_level1.Call
                    Me.m_draw_dom_level2.Call
                    Me.Plot1.Set("OK", Color.White)
                Else
                    Me.Plot1.Set("Level2 data is not avaliable", Color.Red)
                End If
            End If
        End Sub

        Protected Overrides Sub Create()
            Me.m_draw_dom_level1 = New draw_DOM_level(Me)
            Me.m_draw_dom_level2 = New draw_DOM_level(Me)
            Me.Plot1 = MyBase.AddPlot(New StringPlotAttributes("Status", Color.Red))
        End Sub

        Protected Overrides Sub StartCalc()
            Me.m_draw_dom_level1.Data = New Lambda(Of DOMPrice())(Function(x) MyBase.Bars.DOM.Ask)
            Me.m_draw_dom_level1.maxsize = New Lambda(Of Double)(Function(x) Me.m_value11)
            Me.m_draw_dom_level1.From = Color.FromArgb(&HFF, 0, 0)
            Me.m_draw_dom_level1.To = Color.FromArgb(80, 0, 0)
            Me.m_draw_dom_level2.Data = New Lambda(Of DOMPrice())(Function(x) MyBase.Bars.DOM.Bid)
            Me.m_draw_dom_level2.maxsize = New Lambda(Of Double)(Function(x) Me.m_value11)
            Me.m_draw_dom_level2.From = Color.FromArgb(0, 0, &HFF)
            Me.m_draw_dom_level2.To = Color.FromArgb(0, 0, 80)
        End Sub


        ' Properties
        <Input> _
        Public Property updatespeed_seconds As Double


        ' Fields
        Private m_draw_dom_level1 As draw_DOM_level
        Private m_draw_dom_level2 As draw_DOM_level
        Private m_value11 As Double
        Private Plot1 As IPlotObjectStr
    End Class
End Namespace
