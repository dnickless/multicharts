Imports System.Collections.Generic
Imports PowerLanguage
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Namespace PowerLanguage.Indicator
    Public Class vb__Chart_ToolBar_Example_
        Inherits IndicatorObject
        ' Methods
        Public Sub New(ByVal _ctx As Object)
            MyBase.New(_ctx)
            Me.tool_bar_inited = False
            Me.m_plot_color = Color.Red
        End Sub

        Private Sub _new_track(ByVal sender As Object, ByVal e As EventArgs)
            Dim _tsi As TrackBar = DirectCast(sender, TrackBar)
            If (Me.Length <> _tsi.Value) Then
                Me.Length = _tsi.Value
                MyBase.ExecControl.Recalculate()
            End If
        End Sub

        Private Sub AddItem2ToolStrip(ByVal tb As ToolStrip, ByVal item As ToolStripItem)
            item.Tag = Me
            tb.Items.Add(item)
        End Sub

        Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim _tbi As ToolStripButton = DirectCast(sender, ToolStripButton)
            Dim MyDialog As New ColorDialog
            MyDialog.Color = Me.m_plot_color
            If (MyDialog.ShowDialog = DialogResult.OK) Then
                Me.m_plot_color = MyDialog.Color
                vb__Chart_ToolBar_Example_.set_color_tsi(_tbi, MyDialog.Color)
                MyBase.ExecControl.Recalculate()
            End If
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_adx.Set(PublicFunctions.Average(MyBase.Bars.Close, Me.Length), Me.m_plot_color)
        End Sub

        Protected Overrides Sub Create()
            Me.Length = 20
            Me.m_adx = MyBase.AddPlot(New PlotAttributes("Avg"))
        End Sub

        Protected Overrides Sub Destroy()
            If tool_bar_inited Then
                MyBase.ChartToolBar.AccessToolBar(
                    Sub(toolBar)
                        Dim forErase As New List(Of ToolStripItem)
                        For Each item As ToolStripItem In toolBar.Items
                            If (ReferenceEquals(Me, item.Tag)) Then
                                forErase.Add(item)
                            End If
                        Next
                        For Each item As ToolStripItem In forErase
                            toolBar.Items.Remove(item)
                        Next
                    End Sub
                )
            End If
        End Sub

        Private Shared Sub set_color_tsi(ByVal _tsi As ToolStripButton, ByVal _clr As Color)
            Dim _bmp As New Bitmap(50, _tsi.Height)
            Graphics.FromImage(_bmp).FillRectangle(New SolidBrush(_clr), New Rectangle(New Point(0), _bmp.Size))
            _tsi.Image = _bmp
        End Sub

        Protected Overrides Sub StartCalc()
            If Not tool_bar_inited Then                
                MyBase.ChartToolBar.AccessToolBar(
                    Sub(toolBar)
                        Dim tsi3 As New ToolStripButton
                        tsi3.Text = "plot color"
                        set_color_tsi(tsi3, m_plot_color)
                        AddHandler tsi3.Click, AddressOf button1_Click
                        AddItem2ToolStrip(toolBar, tsi3)

                        Dim track As New TrackBar
                        track.Dock = DockStyle.Fill
                        track.Maximum = 1000
                        track.Minimum = 10
                        track.SmallChange = 10
                        track.Value = Length
                        track.Text = "Average Lnegth"

                        AddHandler track.ValueChanged, AddressOf _new_track

                        AddItem2ToolStrip(toolBar, New ToolStripControlHost(track))
                        AddItem2ToolStrip(toolBar, New ToolStripSeparator())
                    End Sub
                )
			tool_bar_inited = True
            End If
        End Sub


        ' Properties
        Private Property Length As Integer


        ' Fields
        Private m_adx As IPlotObject
        Private m_plot_color As Color
        Private tool_bar_inited As Boolean
    End Class
End Namespace
