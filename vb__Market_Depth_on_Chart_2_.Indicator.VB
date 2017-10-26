
Imports PowerLanguage
Imports PowerLanguage.Indicator
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace PowerLanguage.Indicator
    <SameAsSymbol> _
    Public Class vb__Market_Depth_on_Chart_2_
        Inherits IndicatorObject
        Implements IChartCustomDrawer
        ' Methods
        Public Sub New(ByVal ctx As Object)
            MyBase.New(ctx)
            Me.m_volumes = New Dictionary(Of Double, Double)
            Me.m_price_format = "f"
            Me.m_ramka = New Pen(Color.Black, 1!)
            Me.m_ask_bg = New SolidBrush(Color.FromArgb(&HFF, &H80, 0, 0))
            Me.m_bid_bg = New SolidBrush(Color.FromArgb(&HFF, &H19, 50, &H7C))
            Me.m_price_bg = New SolidBrush(Color.FromArgb(&HFF, &H80, &H80, &H80))
            Me.m_ask_clr = New SolidBrush(Color.FromArgb(&HFF, &HFF, 0, 0))
            Me.m_bid_clr = New SolidBrush(Color.FromArgb(&HFF, &H33, &H66, &HFF))
            Me.m_tot_vol_clr = New SolidBrush(Color.Yellow)
            Me.m_str_format = New StringFormat(StringFormat.GenericDefault)
            m_str_format.Alignment = StringAlignment.Center
            Me.UpdateSpeedsec = 0.1
            Me.m_font_size = 10
            Me.m_font_style = FontStyle.Regular
            Me.m_my_font = New Font("Arial", CSng(Me.m_font_size), Me.m_font_style)
            Me.Layout = EDrawPhases.Final
        End Sub
	
		Protected Overrides Sub StopCalc()
            m_last_cb = 0
            m_last_tick_val = 0
            m_volumes.Clear()
            Asks = Nothing
            Bids = Nothing
            ChartCustomDraw.ReDraw()
        End Sub

        Private Sub add_volume(ByVal price As Double, ByVal volume As Double)
            SyncLock Me.m_volumes
                If Not Me.m_volumes.ContainsKey(price) Then
                    Me.m_volumes.Item(price) = volume
                Else
                    m_volumes.Item(price) += volume
                End If
            End SyncLock
        End Sub

        Protected Overrides Sub CalcBar()
            Me.m_last_tick_val = MyBase.Bars.TicksValue
            Me.m_last_cb = MyBase.Bars.CurrentBar
            If Not MyBase.Bars.LastBarOnChart Then
                Dim p_levels As Double = (((MyBase.Bars.HighValue - MyBase.Bars.LowValue) / Me.m_price_step) + 1)
                Dim i As Integer
                For i = 0 To Convert.ToInt32(p_levels) - 1
                    Me.add_volume((MyBase.Bars.LowValue + (i * Me.m_price_step)), (MyBase.Bars.TicksValue / p_levels))
                Next i
            Else
                Dim _inc_volume As Double
                MyBase.ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(Me.UpdateSpeedsec))
                If (MyBase.Bars.CurrentBar > Me.m_last_cb) Then
                    _inc_volume = (MyBase.Bars.TicksValue - Me.m_last_tick_val)
                Else
                    _inc_volume = MyBase.Bars.TicksValue
                End If
                Me.add_volume(MyBase.Bars.CloseValue, _inc_volume)
            End If
        End Sub

        Protected Overrides Sub Create()
            MyBase.ChartCustomDraw.Register(Me)
        End Sub

        Protected Overrides Sub Destroy()
            MyBase.ChartCustomDraw.Unregister(Me)
        End Sub

        Private Sub draw_asks(ByVal asks As DOMPrice(), ByVal _dom_model As dom_model, ByVal _gr As Graphics, ByVal maxTotalVol As Double)
            Dim middle As Integer = Convert.ToInt32(c_levels / 2) - 1

            Dim _max_size As Double = asks.Max(Of Integer)(Function(x) Convert.ToInt32(x.Size))

            Dim i As Integer = middle
            Do While (i > Math.Max(middle - asks.Length, 0))
                Dim _rc As RectangleF = _dom_model.Ask(i)
                Dim _size As Double = asks((middle - i)).Size
                vb__Market_Depth_on_Chart_2_.draw_bg_gradientl2r(Me.m_ask_clr, _gr, _rc, _max_size, _size)
                _gr.DrawString(vb__Market_Depth_on_Chart_2_.Volume2String(_size), Me.m_my_font, If((i = middle), Brushes.White, Brushes.Black), _rc, Me.m_str_format)
                Dim _price As Double = asks((middle - i)).Price
                Dim _rc_p As RectangleF = _dom_model.Price(i)
                vb__Market_Depth_on_Chart_2_.draw_bg_gradientl2r(Me.m_tot_vol_clr, _gr, _rc_p, maxTotalVol, Me.get_volume_for_price(_price))
                _gr.DrawString(vb__Market_Depth_on_Chart_2_.Price2String(_price, Me.m_price_format), Me.m_my_font, Brushes.Black, _rc_p, Me.m_str_format)
                i -= 1
            Loop
        End Sub

        Private Sub draw_bg(ByVal _dom_model As dom_model, ByVal _gr As Graphics)
            _gr.FillRectangle(Me.m_bid_bg, _dom_model.BidRect)
            _gr.FillRectangle(Me.m_price_bg, _dom_model.PriceRect)
            _gr.FillRectangle(Me.m_ask_bg, _dom_model.AskRect)
        End Sub

        Private Shared Sub draw_bg_gradientl2r(ByVal _grad_brush As Brush, ByVal _gr As Graphics, ByVal _rc As RectangleF, ByVal _max As Double, ByVal _val As Double)
            _rc.Width = CSng(((_rc.Width * _val) / _max))
            _gr.FillRectangle(_grad_brush, _rc)
        End Sub

        Private Shared Sub draw_bg_gradientr2l(ByVal _grad_brush As Brush, ByVal _gr As Graphics, ByVal _rc As RectangleF, ByVal _max As Double, ByVal _val As Double)
            Dim _off As Single = CSng(((_rc.Width * _val) / _max))
            _rc.X = (_rc.X + (_rc.Width - _off))
            _rc.Width = _off
            _gr.FillRectangle(_grad_brush, _rc)
        End Sub

        Private Sub draw_bids(ByVal bids As DOMPrice(), ByVal _dom_model As dom_model, ByVal _gr As Graphics, ByVal maxTotalVol As Double)
            Dim middle As Integer = Convert.ToInt32(c_levels / 2)
            Dim _max_size As Double = bids.Max(Of Integer)(Function(x) Convert.ToInt32(x.Size))
            Dim i As Integer
            For i = middle To Math.Min((bids.Length + middle), c_levels) - 1
                Dim _rc As RectangleF = _dom_model.Bid(i)
                Dim _size As Double = bids((i - middle)).Size
                vb__Market_Depth_on_Chart_2_.draw_bg_gradientr2l(Me.m_bid_clr, _gr, _rc, _max_size, _size)
                _gr.DrawString(vb__Market_Depth_on_Chart_2_.Volume2String(_size), Me.m_my_font, If((i = middle), Brushes.White, Brushes.Black), _rc, Me.m_str_format)
                Dim _rc_p As RectangleF = _dom_model.Price(i)
                Dim _price As Double = bids((i - middle)).Price
                vb__Market_Depth_on_Chart_2_.draw_bg_gradientl2r(Me.m_tot_vol_clr, _gr, _rc_p, maxTotalVol, Me.get_volume_for_price(_price))
                _gr.DrawString(vb__Market_Depth_on_Chart_2_.Price2String(_price, Me.m_price_format), Me.m_my_font, Brushes.Black, _rc_p, Me.m_str_format)
            Next i
        End Sub

        Private Sub draw_dom(ByVal asks As DOMPrice(), ByVal bids As DOMPrice(), ByVal graphics As Graphics, ByVal _offset As Point)
            Dim _dom_model As New dom_model(graphics, Me.m_my_font)
            Dim _bmp As New Bitmap((CInt(_dom_model.FullRect.Width) + 6), CInt(_dom_model.FullRect.Height), graphics)
            Using _gr As Graphics = graphics.FromImage(_bmp)
                Me.draw_bg(_dom_model, _gr)
                Dim _max_total_vol As Double = Me.max_volume
                Me.draw_bids(bids, _dom_model, _gr, _max_total_vol)
                Me.draw_asks(asks, _dom_model, _gr, _max_total_vol)
                Me.draw_grid(_dom_model, _gr)
                graphics.DrawImage(_bmp, _offset)
            End Using
        End Sub

        Private Sub draw_grid(ByVal _dom_model As dom_model, ByVal _gr As Graphics)
            _gr.DrawRectangles(Me.m_ramka, _dom_model.AllRects)
            Dim _level As Integer = 1
            Do While (_level <= &H18)
                Dim _lev_rc As RectangleF = _dom_model.Level(_level)
                _gr.DrawLine(Pens.Black, _lev_rc.Location, PointF.Add(_lev_rc.Location, New SizeF(_lev_rc.Width, 0!)))
                _level += 1
            Loop
        End Sub

        Private Shared Function equal(ByVal _1 As DOMPrice, ByVal _2 As DOMPrice) As Boolean
            Return ((_1.Size = _2.Size) AndAlso (_1.Price = _2.Price))
        End Function

        Private Shared Function equal(ByVal _1 As DOMPrice(), ByVal _2 As DOMPrice()) As Boolean
            If Not Object.ReferenceEquals(_1, _2) Then
                If Object.ReferenceEquals(Nothing, _1) Then
                    Return False
                End If
                If Object.ReferenceEquals(Nothing, _2) Then
                    Return False
                End If
                If _1.Equals(_2) Then
                    Return True
                End If
                If (_1.Length <> _2.Length) Then
                    Return False
                End If
                Dim i As Integer
                For i = 0 To _1.Length - 1
                    If Not vb__Market_Depth_on_Chart_2_.equal(_1(i), _2(i)) Then
                        Return False
                    End If
                Next i
            End If
            Return True
        End Function

        Private Function get_volume_for_price(ByVal _price As Double) As Double
            SyncLock Me.m_volumes
                Dim _val As Double
                If Me.m_volumes.TryGetValue(_price, _val) Then
                    Return _val
                End If
                Return 0
            End SyncLock
        End Function

        Protected Overrides Sub OnRecalcLastBarAfterEvent()
            If MyBase.Bars.DOM.Connected Then
                Dim _changed As Boolean = False
                SyncLock Me
                    Dim _old_ask As DOMPrice() = Me.Asks
                    Dim _old_bid As DOMPrice() = Me.Bids
                    Me.Asks = MyBase.Bars.DOM.Ask
                    Me.Bids = MyBase.Bars.DOM.Bid
                    _changed = (Not vb__Market_Depth_on_Chart_2_.equal(_old_ask, Me.Asks) OrElse Not vb__Market_Depth_on_Chart_2_.equal(_old_bid, Me.Bids))
                End SyncLock
                If _changed Then
                    MyBase.ChartCustomDraw.ReDraw
                End If
                MyBase.ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(Me.UpdateSpeedsec))
            End If
        End Sub

        Private Sub Draw(ByVal context As DrawContext, ByVal phase As EDrawPhases) Implements IChartCustomDrawer.Draw
            If (Me.Layout = phase) Then
                Dim asks As DOMPrice()
                Dim bids As DOMPrice()
                SyncLock Me
                    asks = Me.Asks
                    bids = Me.Bids
                End SyncLock
                If ((((Not asks Is Nothing) AndAlso (0 <> asks.Length)) AndAlso ((Not bids Is Nothing) AndAlso (0 <> bids.Length))) AndAlso (context.FullRect = context.DrawRect)) Then
                    Me.draw_dom(asks, bids, context.graphics, New Point(50, 50))
                End If
            End If
        End Sub

        Private Shared Function Price2String(ByVal _price As Double, ByVal price_format As String) As String
            Return _price.ToString(price_format)
        End Function

        Protected Overrides Sub StartCalc()
            Me.m_last_cb = 0
            Me.m_last_tick_val = 0
            Me.m_volumes.Clear
            Me.m_price_step = ((1 / MyBase.Bars.Info.PriceScale) * MyBase.Bars.Info.MinMove)
            Select Case CInt(Math.Log10((1 / MyBase.Bars.Info.PriceScale)))
                Case -6
                    Me.m_price_format = "F6"
                    Exit Select
                Case -5
                    Me.m_price_format = "F5"
                    Exit Select
                Case -4
                    Me.m_price_format = "F4"
                    Exit Select
                Case -3
                    Me.m_price_format = "F3"
                    Exit Select
                Case -2
                    Me.m_price_format = "F2"
                    Exit Select
                Case -1
                    Me.m_price_format = "F1"
                    Exit Select
                Case 0
                    Me.m_price_format = "F0"
                    Exit Select
                Case Else
                    Me.m_price_format = "G"
                    Exit Select
            End Select
        End Sub

        Private Shared Function Volume2String(ByVal _val As Double) As String
            Return CInt(_val).ToString()
        End Function


        ' Properties
        <Input> _
        Public Property Font As String
            Get
                Return Me.m_my_font.FontFamily.Name
            End Get
            Set(ByVal value As String)
                Me.m_my_font = New Font(value, CSng(Me.m_font_size), Me.m_font_style)
            End Set
        End Property

        <Input> _
        Public Property FontSize As Integer
            Get
                Return Me.m_font_size
            End Get
            Set(ByVal value As Integer)
                Me.m_font_size = value
                Me.Font = Me.Font
            End Set
        End Property

        <Input> _
        Public Property FontStyle As FontStyle
            Get
                Return Me.m_font_style
            End Get
            Set(ByVal value As FontStyle)
                Me.m_font_style = value
                Me.Font = Me.Font
            End Set
        End Property

        <Input> _
        Public Property Layout As EDrawPhases

        Private ReadOnly Property max_volume As Double
            Get
                SyncLock Me.m_volumes
                    Return (DirectCast(Me.m_volumes.Values, IEnumerable(Of Double)).Max() * 4)
                End SyncLock
            End Get
        End Property

        <Input> _
        Public Property UpdateSpeedsec As Double


        ' Fields
        Private Asks As DOMPrice()
        Private Bids As DOMPrice()
        Private Const c_levels As Integer = &H18
        Private ReadOnly m_ask_bg As Brush
        Private ReadOnly m_ask_clr As Brush
        Private ReadOnly m_bid_bg As Brush
        Private ReadOnly m_bid_clr As Brush
        Private m_font_size As Integer
        Private m_font_style As FontStyle
        Private m_last_cb As Integer
        Private m_last_tick_val As Double
        Private m_my_font As Font
        Private ReadOnly m_price_bg As Brush
        Private m_price_format As String
        Private m_price_step As Double
        Private ReadOnly m_ramka As Pen
        Private ReadOnly m_str_format As StringFormat
        Private ReadOnly m_tot_vol_clr As Brush
        Private ReadOnly m_volumes As Dictionary(Of Double, Double)

        ' Nested Types
        Private Class dom_model
            ' Methods
            Public Sub New(ByVal graphics As Graphics, ByVal _font As Font)
                Me.m_my_font = _font
                Dim _price_size As SizeF = graphics.MeasureString("11111.2222", Me.m_my_font)
                _price_size.Height = (_price_size.Height + 2!)
                Me.RowHeight = _price_size.Height
                Dim _height As Single = (_price_size.Height * 24!)
                Me.BidRect = New RectangleF(0!, 0!, (_price_size.Width + 2!), _height)
                Me.PriceRect = New RectangleF(Me.BidRect.Width, 0!, (_price_size.Width + 2!), _height)
                Me.AskRect = New RectangleF((Me.BidRect.Width + Me.PriceRect.Width), 0!, (_price_size.Width + 2!), _height)
                Me.FullRect = RectangleF.Union(RectangleF.Union(Me.BidRect, Me.PriceRect), Me.AskRect)
            End Sub

            Public Function Ask(ByVal _level As Integer) As RectangleF
                Dim _lev As RectangleF = Me.Level(_level)
                Return New RectangleF(New PointF(((_lev.Location.X + Me.BidRect.Width) + Me.PriceRect.Width), _lev.Location.Y), New SizeF(Me.AskRect.Width, _lev.Height))
            End Function

            Public Function Bid(ByVal _level As Integer) As RectangleF
                Dim _lev As RectangleF = Me.Level(_level)
                Return New RectangleF(_lev.Location, New SizeF(Me.BidRect.Width, _lev.Height))
            End Function

            Public Function Level(ByVal _level As Integer) As RectangleF
                Return New RectangleF(Me.FullRect.Location.X, (Me.FullRect.Location.Y + (_level * Me.RowHeight)), Me.FullRect.Width, Me.RowHeight)
            End Function

            Public Function Price(ByVal _level As Integer) As RectangleF
                Dim _lev As RectangleF = Me.Level(_level)
                Return New RectangleF(New PointF((_lev.Location.X + Me.BidRect.Width), _lev.Location.Y), New SizeF(Me.PriceRect.Width, _lev.Height))
            End Function


            ' Properties
            Public ReadOnly Property AllRects As RectangleF()
                Get
                    Return New RectangleF() {Me.BidRect, Me.PriceRect, Me.AskRect, Me.FullRect}
                End Get
            End Property

        Property AskRect As RectangleF

        Property BidRect As RectangleF

        Property FullRect As RectangleF

        Property PriceRect As RectangleF

        Property RowHeight As Single


        ' Fields
        Private Const c_x_otstup As Integer = 1
        Private ReadOnly m_my_font As Font
    End Class
    End Class
End Namespace
