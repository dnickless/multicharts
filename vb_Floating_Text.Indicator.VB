Imports PowerLanguage
Imports PowerLanguage.Function
Imports PowerLanguage.Indicator
Imports System
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <RecoverDrawings(False), SameAsSymbol(True)> _
    Public Class vb_Floating_Text
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.textpricepercent = 50
            Me.textbarsback = 10
            Me.texttoplot = "MyText"
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Bars.LastBarOnChart Then
                If (Me.m_text_obj Is Nothing) Then
                    Me.m_text_obj = MyBase.DrwText.Create(New ChartPoint(MyBase.Bars.Time.Item(0), MyBase.Bars.Close.Item(0)), Me.texttoplot)
                Else
                    Me.Text_Float(Me.m_text_obj, Me.textbarsback, Me.textpricepercent)
                End If
            End If
        End Sub


        ' Properties
        <Input> _
        Public Property textbarsback As Integer

        <Input()> _
        Public Property textpricepercent As Double

        <Input()> _
        Public Property texttoplot As String


        ' Fields
        Private m_text_obj As ITextObject
    End Class
End Namespace
