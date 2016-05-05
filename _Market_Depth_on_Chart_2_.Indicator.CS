using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace PowerLanguage.Indicator
{
     [SameAsSymbol]
    public class _Market_Depth_on_Chart_2_ : IndicatorObject, IChartCustomDrawer
    {
        public _Market_Depth_on_Chart_2_(object ctx) :
            base(ctx)
        {
            UpdateSpeedsec = 0.1;
            m_font_size = 10;
            m_font_style = FontStyle.Regular;
            m_my_font = new Font("Arial", m_font_size, m_font_style);
            Layout = EDrawPhases.Final;
        }

        [Input]
        public double UpdateSpeedsec { get; set; }

        protected override void Create(){
            ChartCustomDraw.Register(this);
        }

        protected override void StopCalc()
        {
            m_last_cb = 0;
            m_last_tick_val = 0;
            m_volumes.Clear();
            Asks = null;
            Bids = null;
            ChartCustomDraw.ReDraw();
        }

        protected override void StartCalc()
        {
            m_last_cb = 0;
            m_last_tick_val = 0;
            m_volumes.Clear();
            Asks = null;
            Bids = null;
            m_price_step = (1.0 / Bars.Info.PriceScale) * Bars.Info.MinMove;
            int PS = (int)Math.Log10(1.0 / Bars.Info.PriceScale);
            switch (PS)
            {
                case 0:
                    m_price_format = "F0";
                    break;
                case -1:
                    m_price_format = "F1";
                    break;
                case -2:
                    m_price_format = "F2";
                    break;
                case -3:
                    m_price_format = "F3";
                    break;
                case -4:
                    m_price_format = "F4";
                    break;
                case -5:
                    m_price_format = "F5";
                    break;
                case -6:
                    m_price_format = "F6";
                    break;
                default:
                    m_price_format = "G";
                    break;
            }
        }

        protected override void OnRecalcLastBarAfterEvent(){
            if (Bars.DOM.Connected){
                var _changed = false;
                lock (this){
                    var _old_ask = Asks;
                    var _old_bid = Bids;

                    Asks = Bars.DOM.Ask;
                    Bids = Bars.DOM.Bid;

                    _changed = !(equal(_old_ask, Asks) && equal(_old_bid, Bids));
                }
                
                if (_changed)
                    ChartCustomDraw.ReDraw();
                ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(UpdateSpeedsec));
            }
        }

        private static bool equal(DOMPrice _1, DOMPrice _2){
            return _1.Size == _2.Size && _1.Price == _2.Price;
        }

        private static bool equal(DOMPrice[] _1, DOMPrice[] _2){
            if (ReferenceEquals(_1, _2))
                return true;

            if (ReferenceEquals(null, _1))
                return false;

            if (ReferenceEquals(null, _2))
                return false;

            if (_1.Equals(_2))
                return true;

            if (_1.Length != _2.Length)
                return false;

            for (int i = 0; i < _1.Length; i++){
                if (!equal(_1[i], _2[i]))
                    return false;
            }
            return true;
        }

        protected override void Destroy(){
            ChartCustomDraw.Unregister(this);
        }

        private DOMPrice[] Asks;
        private DOMPrice[] Bids;
        
        private double m_last_tick_val;
        private int m_last_cb;
        private readonly Dictionary<double, double> m_volumes = new Dictionary<double, double>();

        private double m_price_step;
        private string m_price_format = "f";

        protected override void CalcBar(){
            m_last_tick_val = Bars.TicksValue;
            m_last_cb = Bars.CurrentBar;

            if (!Bars.LastBarOnChart){
                var p_levels = (Bars.HighValue - Bars.LowValue)/m_price_step + 1;
                for (var i = 0; i < p_levels; i++)
                    add_volume(Bars.LowValue + i*m_price_step, Bars.TicksValue/p_levels);
                return;
            }

            ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(UpdateSpeedsec));

            double _inc_volume;
            if (Bars.CurrentBar > m_last_cb){
                _inc_volume = Bars.TicksValue - m_last_tick_val;
            }
            else{
                _inc_volume = Bars.TicksValue;
            }
            
            add_volume(Bars.CloseValue, _inc_volume);
        }

        private void add_volume(double price, double volume){
            lock (m_volumes){
                if (!m_volumes.ContainsKey(price))
                    m_volumes[price] = volume;
                else
                    m_volumes[price] += volume;
            }
        }

        private double get_volume_for_price(double _price){
            lock (m_volumes){
                double _val;
                if (m_volumes.TryGetValue(_price, out _val))
                    return _val;
                return 0;
            }
        }

        private double max_volume
        {
            get{
                lock (m_volumes){
                    return m_volumes.Values.Max()*4;
                }
            }
        }

        void IChartCustomDrawer.Draw(DrawContext context, EDrawPhases phase){
            if (Layout != phase)
                return;

            DOMPrice[] asks;
            DOMPrice[] bids;
            lock(this){
                asks = Asks;
                bids = Bids;
            }
            
            if (null == asks || 0 == asks.Length)
                return;
            if (null == bids || 0 == bids.Length)
                return;
            
            if (context.FullRect == context.DrawRect)
                draw_dom(asks, bids, context.graphics, new Point(50, 50));
        }

        private Font m_my_font;
        private int m_font_size;
        private FontStyle m_font_style;

        [Input]
        public EDrawPhases Layout { get; set; }

        [Input]
        public string Font{
            get { return m_my_font.FontFamily.Name; }
            set { m_my_font = new Font(value, m_font_size, m_font_style); }
        }

        [Input]
        public int FontSize {
            get { return m_font_size; }
            set {
                m_font_size = value;
                Font = Font;
            }
        }

        [Input]
        public FontStyle FontStyle
        {
            get { return m_font_style; }
            set
            {
                m_font_style = value;
                Font = Font;
            }
        }

        private static string Price2String(double _price, string price_format){
            return _price.ToString(price_format);
        }

        private static string Volume2String(double _val)
        {
            return ((int)_val).ToString();
        }

        private const int c_levels = 24;

        class dom_model
        {
            private const int c_x_otstup = 1;

            private readonly Font m_my_font;

            public dom_model(Graphics graphics, Font _font){
                m_my_font = _font;

                var _price_size = graphics.MeasureString("11111.2222", m_my_font);
                _price_size.Height += 2;
                RowHeight = _price_size.Height;

                var _height = _price_size.Height * c_levels;

                BidRect = new RectangleF(0, 0, _price_size.Width + 2 * c_x_otstup, _height);
                PriceRect = new RectangleF(BidRect.Width, 0, _price_size.Width + 2 * c_x_otstup, _height);
                AskRect = new RectangleF(BidRect.Width + PriceRect.Width, 0, _price_size.Width + 2 * c_x_otstup, _height);

                FullRect = RectangleF.Union(RectangleF.Union(BidRect, PriceRect), AskRect);
            }
            
            public RectangleF Bid(int _level){
                var _lev = Level(_level);
                _lev = new RectangleF(_lev.Location, new SizeF(BidRect.Width, _lev.Height));
                return _lev;
            }
            public RectangleF Ask(int _level){
                var _lev = Level(_level);
                _lev = new RectangleF(new PointF(_lev.Location.X + BidRect.Width + PriceRect.Width, _lev.Location.Y), new SizeF(AskRect.Width, _lev.Height));
                return _lev;
            }
            public RectangleF Price(int _level){
                var _lev = Level(_level);
                _lev = new RectangleF(new PointF(_lev.Location.X + BidRect.Width, _lev.Location.Y), new SizeF(PriceRect.Width, _lev.Height));
                return _lev;
            }

            public RectangleF Level(int _level){
                return new RectangleF(
                    FullRect.Location.X, FullRect.Location.Y + _level*RowHeight,
                    FullRect.Width, RowHeight);
            }
            
            public float RowHeight { get; private set; }
            public RectangleF FullRect { get; private set; }
            public RectangleF AskRect { get; private set; }
            public RectangleF BidRect { get; private set; }
            public RectangleF PriceRect { get; private set; }

            public RectangleF[] AllRects { get { return new[]{BidRect, PriceRect, AskRect, FullRect}; } }
        }

        private readonly Pen m_ramka = new Pen(Color.Black, 1);
        
        private readonly Brush m_ask_bg = new SolidBrush(Color.FromArgb(255, 128, 0, 0));
        private readonly Brush m_bid_bg = new SolidBrush(Color.FromArgb(255, 25, 50, 124));
        private readonly Brush m_price_bg = new SolidBrush(Color.FromArgb(255, 128, 128, 128));

        private readonly Brush m_ask_clr = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
        private readonly Brush m_bid_clr = new SolidBrush(Color.FromArgb(255, 51, 102, 255));
        private readonly Brush m_tot_vol_clr = new SolidBrush(Color.Yellow);

        private readonly StringFormat m_str_format = new StringFormat(StringFormat.GenericDefault)
                                                     {Alignment = StringAlignment.Center};

        private void draw_dom(DOMPrice[] asks, DOMPrice[] bids, Graphics graphics, Point _offset){
            var _dom_model = new dom_model(graphics, m_my_font);

            var _bmp = new Bitmap((int)_dom_model.FullRect.Width + 6, (int)_dom_model.FullRect.Height, graphics);

            using (var _gr = Graphics.FromImage(_bmp))
            {
                draw_bg(_dom_model, _gr);

                var _max_total_vol = max_volume;
                draw_bids(bids, _dom_model, _gr, _max_total_vol);
                draw_asks(asks, _dom_model, _gr, _max_total_vol);

                draw_grid(_dom_model, _gr);

                graphics.DrawImage(_bmp, _offset);
            }
        }

        private void draw_grid(dom_model _dom_model, Graphics _gr){
            _gr.DrawRectangles(m_ramka, _dom_model.AllRects);

            for (var _level = 1; _level <= c_levels; ++_level){
                var _lev_rc = _dom_model.Level(_level);
                _gr.DrawLine(Pens.Black, _lev_rc.Location, PointF.Add(_lev_rc.Location, new SizeF(_lev_rc.Width, 0)));
            }
        }

        private static void draw_bg_gradientr2l(Brush _grad_brush, Graphics _gr, RectangleF _rc, double _max, double _val){
            var _off = (float) (_rc.Width*_val/_max);
            _rc.X += _rc.Width - _off;
            _rc.Width = _off;
            _gr.FillRectangle(_grad_brush, _rc);
        }

        private static void draw_bg_gradientl2r(Brush _grad_brush, Graphics _gr, RectangleF _rc, double _max, double _val)
        {
            _rc.Width = (float)(_rc.Width * _val / _max);
            _gr.FillRectangle(_grad_brush, _rc);
        }

        private void draw_asks(DOMPrice[] asks, dom_model _dom_model, Graphics _gr, double maxTotalVol){
            const int _middle = c_levels/2 - 1;

            var _max_size = asks.Max(_1 => _1.Size);

            for (var i = _middle; i > Math.Max(_middle - asks.Length, 0); --i){
                var _rc = _dom_model.Ask(i);
                var _size = asks[_middle - i].Size;

                draw_bg_gradientl2r(m_ask_clr, _gr, _rc, _max_size, _size);

                _gr.DrawString(Volume2String(_size), m_my_font, i == _middle ? Brushes.White : Brushes.Black,
                               _rc, m_str_format);

                var _price = asks[_middle - i].Price;
                var _rc_p = _dom_model.Price(i);
                draw_bg_gradientl2r(m_tot_vol_clr, _gr, _rc_p, maxTotalVol, get_volume_for_price(_price));
                _gr.DrawString(Price2String(_price, m_price_format), m_my_font, Brushes.Black, _rc_p, m_str_format);
            }
        }

        private void draw_bids(DOMPrice[] bids, dom_model _dom_model, Graphics _gr, double maxTotalVol){
            const int _middle = c_levels/2;
            var _max_size = bids.Max(_1 => _1.Size);
            for (var i = _middle; i < Math.Min(bids.Length + _middle, c_levels); ++i){
                var _rc = _dom_model.Bid(i);

                var _size = bids[i - _middle].Size;
                draw_bg_gradientr2l(m_bid_clr, _gr, _rc, _max_size, _size);

                _gr.DrawString(Volume2String(_size), m_my_font, i == _middle ? Brushes.White : Brushes.Black,
                               _rc, m_str_format);

                var _rc_p = _dom_model.Price(i);
                var _price = bids[i - _middle].Price;
                draw_bg_gradientl2r(m_tot_vol_clr, _gr, _rc_p, maxTotalVol, get_volume_for_price(_price));
                _gr.DrawString(Price2String(_price, m_price_format), m_my_font, Brushes.Black, _rc_p, m_str_format);
            }
        }

        private void draw_bg(dom_model _dom_model, Graphics _gr){
            _gr.FillRectangle(m_bid_bg, _dom_model.BidRect);
            _gr.FillRectangle(m_price_bg, _dom_model.PriceRect);
            _gr.FillRectangle(m_ask_bg, _dom_model.AskRect);
        }
    }
}