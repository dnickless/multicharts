Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System

Namespace PowerLanguage.Indicator
    <RecoverDrawings(False), MouseEvents> _
    Public Class vb_MouseEventsExample
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal _ctx As Object)
            MyBase.New(_ctx)
        End Sub

        Protected Overrides Sub CalcBar()
        End Sub

        Protected Overrides Sub OnMouseEvent(ByVal arg As MouseClickArgs)
            If (Not Nothing Is Me.m_text) Then
                Me.m_text.Delete
            End If
            Me.m_text = MyBase.DrwText.Create(arg.point, String.Format("user mouse {0} click at bar {1} on data stream {2}", arg.buttons, arg.bar_number, arg.data_number))
        End Sub


        ' Fields
        Private m_text As ITextObject
    End Class
End Namespace
