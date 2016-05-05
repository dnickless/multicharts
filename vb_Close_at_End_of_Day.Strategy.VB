Imports PowerLanguage
Imports System

Namespace PowerLanguage.Strategy
    <IOGMode(IOGMode.Disabled)> _
    Public Class vb_Close_at_End_of_Day
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
        End Sub

        Protected Overrides Sub CalcBar()
            MyBase.GenerateExitOnClose
        End Sub

    End Class
End Namespace
