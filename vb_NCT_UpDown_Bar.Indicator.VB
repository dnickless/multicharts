Imports System
Imports System.Drawing
Imports PowerLanguage
Imports PowerLanguage.Indicator


Namespace PowerLanguage.Indicator
    <SameAsSymbol(True)>
    Public Class vb_NCT_UpDown_Bar
        Inherits IndicatorObject

        Public Sub New(ByVal _ctx As Object)
            MyBase.New(_ctx)
            UpMargin_pt = 5
            UpColor = Color.Green
            DnColor = Color.Red
        End Sub

        Protected Overrides Sub CalcBar()
            If (Bars.Request.Resolution.ChartType = ChartType.Regular Or Bars.Request.Resolution.ChartType = ChartType.HeikinAshi) Then
                Throw New Exception("This study can be applied only to one of the following Non-Standard Chart Types: Point & Figure, Kagi, Line Break, Renko.")
            End If

            Dim ArrowColor As Color = UpColor
            If Bars.UpTicks(0) < Bars.DownTicks(0) Then
                ArrowColor = DnColor
            End If

            plot1.Set(Bars.HighValue + UpMargin_pt * Bars.Info.MinMove / Bars.Info.PriceScale, ArrowColor)
            plot1.BGColor = ArrowColor

        End Sub

        Protected Overrides Sub Create()
            plot1 = MyBase.AddPlot(New PlotAttributes("Up/Down", EPlotShapes.Point, Color.Magenta, Color.Empty, 10, 0, False))
        End Sub

        Protected Overrides Sub StartCalc()
        End Sub

        Protected plot1 As IPlotObject

        <Input>
        Public Property UpMargin_pt As Integer
        <Input>
        Public Property UpColor As Color
        <Input>
        Public Property DnColor As Color


    End Class
End Namespace