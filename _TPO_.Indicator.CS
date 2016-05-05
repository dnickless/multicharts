using System;
using System.Collections.Generic; 
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PowerLanguage.Indicator
{
    sealed class LettersColorsDlg : Form
    {
        public LettersColorsDlg(Dictionary<char, VisualSettings.VisualItem> _letters_colors)
        {
            InitializeComponent();

            dataGridView1.Initialize(_letters_colors);

            Width = 225;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new DataGridViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(268, 491);
            this.dataGridView1.TabIndex = 0;
            // 
            // LettersColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 491);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LettersColorsDlg";
            this.ShowInTaskbar = false;
            this.Text = "Letter\'s color";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewControl dataGridView1;
    }

    sealed class DataGridViewControl : DataGridView
    {
        public DataGridViewControl()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
        }

        internal void Initialize(Dictionary<char, VisualSettings.VisualItem> _letters_colors)
        {
            AutoGenerateColumns = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            RowHeadersVisible = false;

            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.LightGray };

            Columns.Add("Alpha", "Alpha");
            Columns.Add("Color", "Color");

            foreach (var visualItem in _letters_colors)
            {
                var _row = new DataGridViewRow();
                var cell1 = new DataGridViewTextBoxCell { Value = visualItem.Key };
                _row.Cells.Add(cell1);
                cell1.ReadOnly = true;

                var cell2 = new DataGridViewImageCell(false);
                _row.Cells.Add(cell2);

                Update(visualItem.Value, cell2);

                Rows.Add(_row);
            }

            DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private static void Update(VisualSettings.VisualItem visualItem, DataGridViewImageCell cell)
        {
            cell.Tag = visualItem;
            var _bmp = new Bitmap(100, 200);
            var _gr = Graphics.FromImage(_bmp);
            _gr.FillRectangle(new SolidBrush(visualItem.Color), new Rectangle(new Point(0), _bmp.Size));
            cell.Value = _bmp;
        }

        private void DataGridViewCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (-1 != e.RowIndex && 1 == e.ColumnIndex)
            {
                var cell = Rows[e.RowIndex].Cells[e.ColumnIndex];
                var _val = (VisualSettings.VisualItem)cell.Tag;
                _val.ChangeColor();
                Update(_val, (DataGridViewImageCell)cell);
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewControl
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.MultiSelect = false;
            this.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullColumnSelect;
            this.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewCellMouseDoubleClick);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}

namespace PowerLanguage.Indicator
{
    sealed class IDrawContext
    {
        public IDrawContext(ChartPoint point, DrawContext ctx, Color _def_color, DrawSettings _settings,
            double _MaxValue, int _max_periods, float offsetPts){
            DefaultColor = _def_color;
            Settings = _settings;
            m_initial_offset_pts = offsetPts;
            Point = point;
            Ctx = ctx;
            MaxValue = _MaxValue;
            MaxPeriods = _max_periods;
        }

        public DrawSettings Settings { get; private set; }

        public ChartPoint Point { get; private set; }

        public DrawContext Ctx { get; private set; }

        public double MaxValue { get; private set; }

        public int MaxPeriods { get; private set; }

        private readonly float m_initial_offset_pts;
        private float m_offset_pts;
        public float OffsetPts {
            get { return m_offset_pts + m_initial_offset_pts; }
            set{
                LastOffsetPts = value;
                m_offset_pts = Math.Max(m_offset_pts, value);
            }
        }

        public float PureLastOffsetPts { get { return m_last_offset_pts; } }
        private float m_last_offset_pts;
        public float LastOffsetPts{
            get { return m_last_offset_pts + m_initial_offset_pts; }
            set { m_last_offset_pts = value; }
        }

        public Brush SelectColor(int _ch){
            return SelectColor(AlphasByPrices.num2char(_ch), false);
        }

        public Brush SelectColor(char _ch, bool first){
            var _color = Settings.VisualSettings.SelectColor(_ch);
            if (first && Settings.VisualSettings.OpenVisible)
                _color = Settings.VisualSettings.OpenColor;
            return new SolidBrush(_color);
        }
        public Brush SelectColor(IPerPriceLevelItem _item, bool _first, int _cur_item, int _max_items)
        {
            return new SolidBrush(select_color(_item, _first, _cur_item, _max_items));
        }

        private Color select_color(IPerPriceLevelItem _item, bool _first, int _cur_item, int _max_items)
        {
            if (_item.First && _first && Settings.VisualSettings.OpenVisible)
                return Settings.VisualSettings.OpenColor;
            if (_item.ValueArea && !_item.Max)
                return Gradient(Settings.VisualSettings.ValueAreaColor, _cur_item, _max_items);
            return Gradient(DefaultColor, _cur_item, _max_items);
        }

        Color DefaultColor { get; set; }

        private static byte correct(byte _src, float _coeff)
        {
            const float _part = 1.0f / 3.0f;
            return (byte)(_src * (_part + (1 - _part) * _coeff));
        }

        private static Color Gradient(Color _src, int curItem, int maxItems)
        {
            var _coeff = (float)curItem / maxItems;
            return Color.FromArgb(_src.A, correct(_src.R, _coeff), correct(_src.G, _coeff), correct(_src.B, _coeff));
        }
    }

    interface IDrawable
    {
        void Draw(IDrawContext ctx);
        int Height(IDrawContext ctx);
        int Height(DrawContext ctx, DrawSettings sett);
    }

    [Serializable]
    [XmlTypeAttribute]
    [XmlRootAttribute]
    public class VisualSettings
    {
        [Serializable]
        [XmlTypeAttribute]
        public class VisualItem
        {
            public VisualItem(Color color, string _name, _TPO_ mOwner, bool _check_readonly){
                Color = color;
                Visible = true;
                Name = _name;

                m_checkbox_RO = _check_readonly;
                m_owner = mOwner;
            }

            public VisualItem(){}

            private void SettingsChanged(){
                if (null!=m_owner)
                    m_owner.SettingsChanged();
            }

            private Color m_color;

            [XmlAttribute]
            public int ColorSerialize
            {
                set { Color = Color.FromArgb(value); }
                get { return Color.ToArgb(); }
            }

            [XmlIgnore]
            public Color Color{
                get { return m_color; }
                private set{
                    if (m_color != value)
                        SettingsChanged();
                    m_color = value;
                }
            }

            private bool m_visible;
            [XmlAttribute]
            public bool Visible{
                get { return m_visible; }
                set{
                    if (m_visible != value)
                        SettingsChanged();
                    m_visible = value;
                }
            }

            [XmlAttribute]
            public bool m_checkbox_RO;

            [NonSerialized]
            private _TPO_ m_owner;

            [XmlAttribute]
            public string Name { get; set; }

            public ToolStripItem TSItem{
                get{
                    var _chb = new CheckBox{Text = Name, BackColor = Color, Checked = Visible, AutoCheck = false};
                    _chb.MouseClick +=(_1,_2)=>{
                        var _check_rc = new Rectangle(new Point(0), new Size(_chb.Size.Height, _chb.Size.Height));
                        if (_check_rc.Contains(_2.Location))
                        {
                            if (!m_checkbox_RO)
                                _chb.Checked = !_chb.Checked;
                        }
                        else
                        {
                            ChangeColor();
                            _chb.BackColor = Color;
                        }
                    };
                    _chb.CheckedChanged += (_1, _2) => { Visible = _chb.Checked; };
                    _chb.Width = 40;
                    var _ti = new ToolStripControlHost( _chb );
                    return _ti;
                }
            }

            internal _TPO_ Owner{
                set { m_owner = value; }
            }

            internal void ChangeColor(){
                var MyDialog = new ColorDialog{Color = Color};
                if (MyDialog.ShowDialog() == DialogResult.OK)
                    Color = MyDialog.Color;
            }
        }

        [NonSerialized]
        private Dictionary<string, VisualItem> m_items = new Dictionary<string, VisualItem>();

        private Dictionary<char, VisualItem> m_lc = new Dictionary<char, VisualItem>();

        [XmlType]
        public class Pair<TFirst, TSecond>
        {
            public Pair(){
            }

            public Pair(TFirst first, TSecond second){
                First = first;
                Second = second;
            }

            [XmlElement]
            public TFirst First { get; set; }

            [XmlElement]
            public TSecond Second { get; set; }
        }

        [XmlElement]
        public Pair<char, VisualItem>[] LetterColorsSerialize
        {
            get { return m_lc.Select(_1 => new Pair<char, VisualItem>(_1.Key, _1.Value)).ToArray(); }
            set { m_lc = value.ToDictionary(_1 => _1.First, _1 => _1.Second); }
        }

        [XmlElement]
        public Pair<string, VisualItem>[] ItemsColorsSerialize
        {
            get { return m_items.Select(_1 => new Pair<string, VisualItem>(_1.Key, _1.Value)).ToArray(); }
            set { m_items = value.ToDictionary(_1 => _1.First, _1 => _1.Second); }
        }

        [NonSerialized]
        private _TPO_ m_owner;

        public VisualSettings():this(null){}

        private void fill_letters(char _from, char _to){
            for (var _i = _from; _i <= _to; ++_i)
                m_lc[_i] = new VisualItem(Color.FromKnownColor((KnownColor) (_i - _from + 0x1c)),
                                          _i.ToString(CultureInfo.InvariantCulture), m_owner, false);
        }

        private VisualSettings(_TPO_ _owner){
            if (null==_owner)
                return;

            CreateAfterSerialize(_owner);

            fill_items(true);

            fill_letters('A', 'Z');
            fill_letters('a', 'z');
        }
        
        private void add_item(string name, VisualItem item, bool _force){
            if (_force || !m_items.ContainsKey(name))
                m_items[name] = item;
        }

        private void fill_items(bool _force){
            add_item("Default", new VisualItem(Color.Green, "TPO", m_owner, true), _force);
            add_item("MaxItem", new VisualItem(Color.Magenta, "POC", m_owner, true), _force);
            add_item("Open", new VisualItem(Color.Orange, "Open", m_owner, false), _force);
            add_item("ValueArea", new VisualItem(Color.CornflowerBlue, "Value Area", m_owner, true), _force);
            add_item("Close", new VisualItem(Color.Red, "Close", m_owner, false), _force);
            add_item("ValueAreaRange", new VisualItem(Color.CornflowerBlue, "VA Range", m_owner, false), _force);
            add_item("POCLine", new VisualItem(Color.Magenta, "POC Line", m_owner, false), _force);
            add_item("IBR", new VisualItem(Color.Cyan, "IBR", m_owner, false), _force);
        }

        internal void save(){
            if (null == m_owner)
                return;

            using (var stream = new MemoryStream()){
                new XmlSerializer(typeof (VisualSettings)).Serialize(stream, this);
                m_owner.PersistData = Convert.ToBase64String(stream.GetBuffer());
            }
        }

        private static readonly string s_stg_file = Application.LocalUserAppDataPath + "/TPO_settings.xml";

        internal static VisualSettings load(_TPO_ _owner){
            try
            {
                var _data = _owner.PersistData;
                if (!string.IsNullOrEmpty(_data))
                    return load(_owner, new MemoryStream(Convert.FromBase64String(_data)));

                using (var stream = File.Open(s_stg_file, FileMode.Open))
                    return load(_owner, stream);

            }catch (Exception e){
                Trace.TraceError("Error occured during load data from file {0}:{1}", s_stg_file, e);
            }
            return new VisualSettings(_owner);
        }

        private static VisualSettings load(_TPO_ _owner, Stream stream){
            var _result = (VisualSettings) new XmlSerializer(typeof (VisualSettings)).Deserialize(stream);
            _result.fill_items(false);
            _result.CreateAfterSerialize(_owner);
            return _result;
        }

        protected void CreateAfterSerialize(_TPO_ _owner){
            m_owner = _owner;

            foreach (var item in m_items.Values)
                item.Owner = m_owner;

            foreach (var item in m_lc.Values)
                item.Owner = m_owner;
        }

        private void SettingsChanged()
        {
            if (null != m_owner)
                m_owner.SettingsChanged();
        }

        private bool m_draw_volume;
        [XmlAttribute]
        public bool DrawVolume
        {
            get { return m_draw_volume; }
            set
            {
                if (m_draw_volume != value)
                {
                    m_draw_volume = value;
                    SettingsChanged();
                }
            }
        }

        private bool m_draw_tpo = true;
        [XmlAttribute]
        public bool DrawTPO
        {
            get { return m_draw_tpo; }
            set
            {
                if (m_draw_tpo != value)
                {
                    m_draw_tpo = value;
                    SettingsChanged();
                }
            }
        }

        public Color SelectColor(char _alpha){
            return m_lc[_alpha].Color;
        }

        [XmlAttribute]
        public bool DrawNumOfChars{
            get { return m_draw_num_of_chars; }
            set{
                if (m_draw_num_of_chars != value)
                {
                    m_draw_num_of_chars = value;
                    SettingsChanged();
                }
            }
        }

        [XmlAttribute]
        public _TPO_.ETPOColoring ColoringShema{
            get { return m_coloring_shema; }

            set{
                if (m_coloring_shema != value){
                    m_coloring_shema = value;
                    SettingsChanged();
                }
            }
        }

        public Color DefaultColor { get { return m_items["Default"].Color; } }
        public Color MaxItemColor { get { return m_items["MaxItem"].Color; } }
        public Color OpenColor { get { return m_items["Open"].Color; } }
        public Color ValueAreaColor { get { return m_items["ValueArea"].Color; } }
        public Color CloseColor { get { return m_items["Close"].Color; } }
        public Color ValueAreaRangeColor { get { return m_items["ValueAreaRange"].Color; } }
        public Color POCLineColor { get { return m_items["POCLine"].Color; } }
        public Color IBRColor { get { return m_items["IBR"].Color; } }

        public bool DefaultVisible { get { return m_items["Default"].Visible; } }
        public bool MaxItemVisible { get { return m_items["MaxItem"].Visible; } }
        public bool OpenVisible { get { return m_items["Open"].Visible; } }
        public bool ValueAreaVisible { get { return m_items["ValueArea"].Visible; } }
        public bool CloseVisible { get { return m_items["Close"].Visible; } }
        public bool ValueAreaRangeVisible { get { return m_items["ValueAreaRange"].Visible; } }
        public bool POCLineVisible { get { return m_items["POCLine"].Visible; } }
        public bool IBRVisible { get { return m_items["IBR"].Visible; } }

        private bool m_highlighted_letters = true;
        [XmlAttribute]
        public bool HighlightedLetters
        {
            get { return m_highlighted_letters; }
            set
            {
                if (m_highlighted_letters != value)
                {
                    m_highlighted_letters = value;
                    SettingsChanged();
                }
            }
        }

        private float m_Offset2Right;

        [Serializable]
        public class FontWrap
        {
            public FontWrap(){
                Value = SystemFonts.DefaultFont;
            }

            [XmlAttribute]
            public string FontSerializeableContent {
                get{
                    var _mem_stream = new MemoryStream();
                    new BinaryFormatter().Serialize(_mem_stream, Value);
                    return Convert.ToBase64String(_mem_stream.GetBuffer());
                }
                set{
                    using (var _mem_stream = new MemoryStream(Convert.FromBase64String(value)))
                        Value = (Font) new BinaryFormatter().Deserialize(_mem_stream);
                }
            }

            public Font Value { get; set; }

            public override string ToString(){
                return string.Format("{0}({1})", Value.Name, Value.Size);
            }
        }

        [XmlAttribute]
        public string FontSerializeableContent{
            get { return m_font.FontSerializeableContent; }
            set { m_font.FontSerializeableContent = value; }
        }

        private FontWrap m_font = new FontWrap();
        private _TPO_.ETPOColoring m_coloring_shema = _TPO_.ETPOColoring.ByPrice;
        private bool m_draw_num_of_chars;

        [XmlAttribute]
        public float Offset2Right{
            get {
                return m_Offset2Right;
            }
            set {
                if (m_Offset2Right != value)
                {
                    m_Offset2Right = value;
                    SettingsChanged();
                }
            }
        }

        internal string FontStr { get { return m_font.ToString(); } }

        internal Font TextFont{
            get { return m_font.Value; }
            set{
                m_font.Value = value;
                SettingsChanged();
            }
        }

        internal ToolStripItem[] CreateControls(string prof_type){
            var _result = new List<ToolStripItem>();

            {
                var _ch_b = new CheckBox { Checked = DrawNumOfChars, Text = "Draw Level Value" };
                _ch_b.CheckedChanged += (_1, _2) => { DrawNumOfChars = _ch_b.Checked; };
                _result.Add(new ToolStripControlHost(_ch_b));
            }
            
            {
                var _ts = new ToolStripDropDownButton("Colors && Appearance");
                foreach (var value in m_items.Values)
                    _ts.DropDownItems.Add(value.TSItem);

                _result.Add(_ts);
            }

            if (prof_type == _TPO_.c_prof_t_alphas)
            {
                var _ts = new ToolStripButton("Colors");
                _ts.Click += (_1, _2) => new LettersColorsDlg(m_lc).ShowDialog();
                _result.Add(_ts);
            }
            return _result.ToArray();
        }
    }

    internal sealed class DrawSettings
    {
        public DrawSettings(VisualSettings visualSettings)
        {
            VisualSettings = visualSettings;
        }

        public VisualSettings VisualSettings { get; private set; }

        public bool DrawPriceLines { get; internal set; }
    }

    class PriceCluster<T> where T : class, IDrawable, IPerPriceLevelItem, new()
    {
        private readonly SortedDictionary<Price, T> m_data = new SortedDictionary<Price, T>();

        public SortedDictionary<Price, T> Data
        {
            get { return m_data; }
        }

        public KeyValuePair<Price, Price>? IBR { get; set; }

        public DateTime Start{
            get { return m_start; }
        }

        private readonly PriceCtx m_ctx;
        private readonly DateTime m_start;
        private readonly DateTime m_end;
        private readonly Session m_session;
        private T m_max_item;
        private int m_max_periods;

        public PriceCluster(PriceCtx mCtx, Session sess){
            m_session = sess;
            m_ctx = mCtx;
            m_start = sess.Begin;
            m_end = sess.End;
        }

        public T this[double _price]
        {
            get
            {
                var _key = new Price(_price, m_ctx);
                if (!m_data.ContainsKey(_key))
                {
                    var _first = 0 == m_data.Count;
                    var _result = m_data[_key] = new T();
                    if (_first)
                        _result.First = true;
                    return _result;
                }
                return m_data[_key];
            }
        }

        public void Draw(DrawContext ctx, ref float _offset_points, DrawSettings _settings)
        {
            if (0 == Data.Count)
                return;

            if (_settings.DrawPriceLines)
                draw_IBR(ctx, _offset_points, _settings);
            draw_levels(ctx, ref _offset_points, _settings);
        }

        private void draw_IBR(DrawContext ctx, float offsetPoints, DrawSettings settings)
        {
            if (!IBR.HasValue)
                return;
            if (!settings.VisualSettings.IBRVisible)
                return;

            var _IBR = IBR.Value;
            var env = ctx.Environment;
            var _bottom = env.ChartPoint2Point(new ChartPoint(m_start, _IBR.Key.Dbl));
            var _top = env.ChartPoint2Point(new ChartPoint(m_start, _IBR.Value.Dbl));
            var _h = _bottom.Y - _top.Y;

            var _offset = new SizeF(2 - offsetPoints, 0);
            _top -= _offset;
            var _rect = new RectangleF(_top, new SizeF(3, _h));
            ctx.graphics.FillRectangle(new SolidBrush(settings.VisualSettings.IBRColor), _rect);
        }

        private void draw_levels(DrawContext ctx, ref float offsetPoints, DrawSettings settings)
        {
            /*
            var _first = Data.First();
            var _last = Data.Last();

            var env = ctx.Environment;
            
            var _bottom = env.ChartPoint2Point(new ChartPoint(m_start, _first.Key.Dbl));
            var _top = env.ChartPoint2Point(new ChartPoint(m_start, _last.Key.Dbl));

            var _num = (int)(_bottom.Y - _top.Y) / (_first.Value.Height(ctx, settings));
            var _levels = (int)(_last.Key.Int - _first.Key.Int) / m_ctx.MinMove;
            if (_levels <= _num)
                _num = 1;
            else
                _num = (int) ((float) _levels/_num + 1.0f);
            
            var i = 0;
            */

            var _value_area_start = false;
            Price _va_begin = null, _va_end = null, _max_price = null, _prev = null;
            float _va_begin_off = 0, _va_end_off = 0, _prev_off = 0;

            var _initial_offset_pts = offsetPoints;
            IDrawContext _draw_ctx = null;
            foreach (var price_level in Data)
            {
                var _force_draw = false;

                if (!_value_area_start && price_level.Value.ValueArea){
                    _value_area_start = true;
                    _va_begin = price_level.Key;
                }

                if (_value_area_start && !price_level.Value.ValueArea){
                    _value_area_start = false;
                    _va_end = _prev;
                }

                if (price_level.Value.Max){
                    _max_price = price_level.Key;
                    _force_draw = true;
                }

                if (/*0 == i++ % _num*/ true || _force_draw)
                {
                    _draw_ctx = new IDrawContext(
                        new ChartPoint(m_start, price_level.Key.Dbl), ctx,
                        price_level.Value.Max ? settings.VisualSettings.MaxItemColor : settings.VisualSettings.DefaultColor,
                        settings,
                        m_max_item.Value, m_max_periods, _initial_offset_pts);
                    price_level.Value.Draw(_draw_ctx);

                    offsetPoints = Math.Max(offsetPoints, _draw_ctx.OffsetPts);
                }

                if (_va_begin == price_level.Key)
                    _va_begin_off = _draw_ctx.LastOffsetPts;

                if (_va_end == _prev)
                    _va_end_off = _prev_off;

                _prev = price_level.Key;
                if (null != _draw_ctx)
                    _prev_off = _draw_ctx.LastOffsetPts;
            }

            if (settings.DrawPriceLines){
                offsetPoints += 100;
                if (settings.VisualSettings.ValueAreaRangeVisible)
                {
                    draw_price_line(ctx, _va_begin_off, offsetPoints, _va_begin,
                                    settings.VisualSettings.ValueAreaRangeColor, settings.VisualSettings.TextFont);
                    draw_price_line(ctx, _va_end_off, offsetPoints, _va_end, settings.VisualSettings.ValueAreaRangeColor,
                                    settings.VisualSettings.TextFont);
                }
                if (settings.VisualSettings.POCLineVisible)
                    draw_price_line(ctx, offsetPoints - 100, offsetPoints, _max_price,
                                    settings.VisualSettings.POCLineColor, settings.VisualSettings.TextFont);
            }
        }

        private void draw_price_line(DrawContext ctx, float offsetPoints, float maxoffsetPoints,
            Price _price, Color _clr, Font _font)
        {
            if (null==_price)
                return;

            var _str_price = _price.ToString();
            var _price_size = ctx.graphics.MeasureString(_str_price, _font);
            var _start_p = ctx.Environment.ChartPoint2Point(new ChartPoint(m_start, _price.Dbl));
            _start_p.X += offsetPoints;
            var line_size = maxoffsetPoints - offsetPoints;
            var _line_end = _start_p;
            _line_end.X += line_size - _price_size.Width - 2;
            ctx.graphics.DrawLine(new Pen(_clr, 1), _start_p, _line_end);
            _line_end.X += 2;
            _line_end.Y -= _font.Height/2f;
            ctx.graphics.DrawString(_str_price, _font, new SolidBrush(_clr), _line_end);
        }

        public void NextPeriod()
        {
            foreach (var items in m_data.Values)
                items.NextPeriod();
        }

        public void PostProcess(int periodNum)
        {
            FindMax();
            CalcValueArea();
            m_max_periods = periodNum;
        }

        private void CalcValueArea()
        {
            var _array = m_data.Values.ToList();

            const double Percent = 0.7;

            var _total_volume = _array.Sum(_1 => _1.Value) * Percent;
            var _cur_volume = .0;

            var _best_item = _array.IndexOf(m_max_item);
            Trace.Assert(0 > _best_item);
            _array[_best_item].ValueArea = true;

            var _up = _best_item - 1;
            var _down = _best_item + 1;
            while (_cur_volume < _total_volume && !(_up < 0 && _down >= _array.Count))
            {
                var _up_volume = .0;
                var _down_volume = .0;
                if (_up >= 0)
                    _up_volume = _array[_up].Value;
                if (_down < _array.Count)
                    _down_volume = _array[_down].Value;

                if (_up_volume > _down_volume)
                    calcVAmove_up(_array, ref _up, ref _cur_volume);
                else if (_up_volume < _down_volume)
                    calcVAmove_down(_array, ref _down, ref _cur_volume);
                else
                {
                    if (_best_item - _up < _down - _best_item)
                        calcVAmove_up(_array, ref _up, ref _cur_volume);
                    else
                        calcVAmove_down(_array, ref _down, ref _cur_volume);
                }
            }
        }

        private static void calcVAmove_down(IList<T> _array, ref int _down, ref double _cur_volume)
        {
            var _item = _array[_down];
            _cur_volume += _item.Value;
            _item.ValueArea = true;
            _down++;
        }

        private static void calcVAmove_up(IList<T> _array, ref int _up, ref double _cur_volume)
        {
            var _item = _array[_up];
            _cur_volume += _item.Value;
            _item.ValueArea = true;
            _up--;
        }

        private void FindMax(){
            m_max_item = null;
            if (0==m_data.Count)
                return;

            var _max = double.MinValue;
            var _max_price = double.MinValue;
            var _max_from_middle_dif_abs = double.MaxValue;

            var _middle = (m_data.Keys.Last().Dbl + m_data.Keys.First().Dbl) / 2;

            foreach (var _t in m_data)
            {
                var _val = _t.Value.Value;
                var _new_max = false;
                var _cur_price = _t.Key.Dbl;
                var _cur_diff = Math.Abs(_middle - _cur_price);

                if (_val > _max){
                    _new_max = true;
                }else if (_val == _max){
                    if (_cur_diff < _max_from_middle_dif_abs)
                        _new_max = true;
                    else if (_cur_diff == _max_from_middle_dif_abs)
                        _new_max = _cur_price < _max_price;
                }

                if (_new_max){
                    _max = _val;
                    _max_price = _cur_price;
                    _max_from_middle_dif_abs = _cur_diff;
                    m_max_item = _t.Value;
                }
            }
            if (m_max_item != null)
                m_max_item.Max = true;
        }

        public bool IsMySession(Session session){
            return ReferenceEquals(session, m_session);
        }

        public bool IsMyInterval(DateTime start, DateTime end){
            if (start <= m_start && m_start <= end)
                return true;
            if (start <= m_end && m_end <= end)
                return true;
            return false;
        }
    }

    interface IPerPriceLevelItem
    {
        void Hit(float totalVolume, int _period_num);
        void NextPeriod();
        double Value { get; }

        bool First { set; get; }
        bool Last { set; get; }
        bool ValueArea { set; get; }
        bool Max { set; get; }
    }

    abstract class ItemByPrices : IDrawable, IPerPriceLevelItem
    {
        protected abstract void on_hit(int _period_num);

        private bool m_touched;
        protected double m_total_volume;

        public void Hit(float totalVolume, int _period_num)
        {
            m_total_volume += totalVolume;
            if (!m_touched)
            {
                m_touched = true;
                on_hit(_period_num);
            }
        }

        public void NextPeriod()
        {
            m_touched = false;
        }

        public abstract double Value { get; }

        public bool First { get; set; }
        public bool Last { get; set; }
        public bool ValueArea { get; set; }
        public bool Max { set; get; }

        public abstract void Draw(IDrawContext ctx);
        public abstract int Height(DrawContext ctx, DrawSettings settings);
        public int Height(IDrawContext drawContext){
            return Height(drawContext.Ctx, drawContext.Settings);
        }

        protected void draw(IDrawContext ctx, int _h, PointF _pt_g, SizeF my_size){
            if (Last && ctx.Settings.VisualSettings.CloseVisible)
            {
                var _rect = new SizeF(_h / 2f, _h / 2f);
                var _end = _pt_g + my_size + new SizeF(3, -my_size.Height/2);
                ctx.Ctx.graphics.FillPolygon(new SolidBrush(ctx.Settings.VisualSettings.CloseColor),
                                             new[]{
                                                      _end, new PointF(_end.X + _rect.Height, _end.Y - _rect.Height/2),
                                                      new PointF(_end.X + _rect.Height, _end.Y + _rect.Height/2)
                                                  });
                ctx.OffsetPts = _end.X + _rect.Height - _pt_g.X;
            }

            if (ctx.Settings.VisualSettings.DrawNumOfChars)
            {
                var str = Value.ToString("G");
                var _pt = _pt_g - new SizeF(10 + ctx.Ctx.graphics.MeasureString(str, ctx.Settings.VisualSettings.TextFont).Width, 0);
                ctx.Ctx.graphics.DrawString(str, ctx.Settings.VisualSettings.TextFont, Brushes.White, _pt);
            }
        }
    }

    sealed class AlphasByPrices : ItemByPrices
    {
        private readonly StringBuilder m_my_string = new StringBuilder();

        protected override void on_hit(int _period)
        {
            m_my_string.Append(num2char(_period));
        }

        public override double Value
        {
            get { return MyString.Length; }
        }

        public static char num2char(int _num)
        {
            const int _count = 'Z' - 'A';
			_num = _num % (2 * _count);
            if (_count <= _num)
                return (char)('a' + (char)(_num - _count));
            return (char)('A' + _num);
        }

        private string MyString
        {
            get
            {
                return m_my_string.ToString();
            }
        }

        public override void Draw(IDrawContext ctx)
        {
            var str = MyString;
            if (0 == str.Length)
                return;

            var _env = ctx.Ctx.Environment;
            int _h = Height(ctx);
            var _pt_g = _env.ChartPoint2Point(ctx.Point) - new SizeF(-5 - ctx.OffsetPts, _h / 2f);

            if (_TPO_.ETPOColoring.ByPrice == ctx.Settings.VisualSettings.ColoringShema)
                draw_price_coloring(ctx, _pt_g, str);
            else
                draw_time_coloring(ctx, _pt_g, str);

            var my_size = new SizeF(ctx.PureLastOffsetPts, _h);

            draw(ctx, _h, _pt_g, my_size);
        }

        private void draw_time_coloring(IDrawContext ctx, PointF ptG, string str){
            var i = 0;
            var _ch_size = ctx.Ctx.graphics.MeasureString("A", ctx.Settings.VisualSettings.TextFont);
            foreach (var ch in str){
                var _pt = ptG;
                _pt.X += i*_ch_size.Width;

                if (ctx.Settings.VisualSettings.HighlightedLetters){
                    ctx.Ctx.graphics.DrawString(ch.ToString(CultureInfo.InvariantCulture),
                                                ctx.Settings.VisualSettings.TextFont, ctx.SelectColor(ch, false), _pt);
                }
                else
                {
                    ctx.Ctx.graphics.FillRectangle(ctx.SelectColor(ch, false), new RectangleF(_pt, _ch_size));
                    ctx.Ctx.graphics.DrawString(ch.ToString(CultureInfo.InvariantCulture),
                                                ctx.Settings.VisualSettings.TextFont, Brushes.Black, _pt);
                }

                i++;
            }
            ctx.OffsetPts = _ch_size.Width * i;
        }

        private void draw_price_coloring(IDrawContext ctx, PointF _pt_g, string str){
            ctx.Ctx.graphics.DrawString(str, ctx.Settings.VisualSettings.TextFont, ctx.SelectColor(this, false, 1, 1), _pt_g);
            ctx.OffsetPts = ctx.Ctx.graphics.MeasureString(str, ctx.Settings.VisualSettings.TextFont).Width;
        }

        public override int Height(DrawContext ctx, DrawSettings settings)
        {
            return (int)ctx.graphics.MeasureString(" ", settings.VisualSettings.TextFont).Height + 2;
        }
    }

    sealed class SquareByPrices : ItemByPrices
    {
        private int m_my_rects;
        private readonly List<int> m_period_num = new List<int>();

        protected override void on_hit(int _period)
        {
            m_my_rects++;
            m_period_num.Add(_period);
        }

        public override double Value
        {
            get { return m_my_rects; }
        }

        public override void Draw(IDrawContext ctx)
        {
            if (0 == m_my_rects)
                return;

            int _h = Height(ctx);
            var _rect = new SizeF(_h, _h);
            var _pt_g = ctx.Ctx.Environment.ChartPoint2Point(ctx.Point) - new SizeF(-5 - ctx.OffsetPts, _rect.Height / 2);
            var _graphics = ctx.Ctx.graphics;

            for (var i = 0; i < m_my_rects; i++)
            {
                var cur_pt = _pt_g;
                cur_pt.X += i * _rect.Width * 1.1f + 1;
                _graphics.FillRectangle(ctx.SelectColor(this, 0 == i, m_period_num[i], ctx.MaxPeriods), new RectangleF(cur_pt, _rect));
            }

            var my_size = new SizeF(m_my_rects * _rect.Width * 1.1f + 1, _rect.Height);
            ctx.OffsetPts = my_size.Width;

            draw(ctx, _h, _pt_g, my_size);
        }

        public override int Height(DrawContext ctx, DrawSettings settings)
        {
            return settings.VisualSettings.TextFont.Height + 2;
        }
    }

    sealed class VolumeByPrice : ItemByPrices
    {
        protected override void on_hit(int _period_num) { }

        public override double Value
        {
            get { return m_total_volume; }
        }

        public override void Draw(IDrawContext ctx)
        {
            if (0 == m_total_volume)
                return;

            const int max_pixels = 150;
            float _h = Height(ctx);

            RectangleF _my_rect;
            if (true) 
            {
                var _pt_g = ctx.Ctx.Environment.ChartPoint2Point(ctx.Point) + new SizeF(5 + ctx.OffsetPts + max_pixels, _h / 2);
                var _my_size = new SizeF((float)(max_pixels * (Value / ctx.MaxValue)), _h);
                _pt_g.X = _pt_g.X - _my_size.Width;
                _my_rect = new RectangleF(_pt_g, _my_size);
            }
            ctx.Ctx.graphics.FillRectangle(ctx.SelectColor(this, false, 1, 1), _my_rect);
            var _str_format = new StringFormat{Alignment = StringAlignment.Far};
            ctx.Ctx.graphics.DrawString(m_total_volume.ToString("G"), ctx.Settings.VisualSettings.TextFont, Brushes.Black, _my_rect,
                                        _str_format);
        }

        public override int Height(DrawContext ctx, DrawSettings settings)
        {
            return settings.VisualSettings.TextFont.Height + 2;
        }
    }

    internal interface IPriceClusters
    {
        void NewCluster(Session session, TimeSpan _period, TimeSpan ibrPeriod);
        void Draw(DrawContext _ctx, DrawSettings _settings);
    }

    sealed class PriceClusterses<TPriceProfItem, TVolumeProfItem> : IPriceClusters
        where TPriceProfItem : class, IDrawable, IPerPriceLevelItem, new()
        where  TVolumeProfItem : class, IDrawable, IPerPriceLevelItem, new()
    {
        private readonly SortedDictionary<DateTime, PriceCluster<TPriceProfItem>> m_price_profile = new SortedDictionary<DateTime, PriceCluster<TPriceProfItem>>();
        private readonly SortedDictionary<DateTime, PriceCluster< TVolumeProfItem>> m_volume_profile = new SortedDictionary<DateTime, PriceCluster< TVolumeProfItem>>();

        private readonly PriceCtx m_ctx;
        private readonly bool m_build_from_bar;

        public PriceClusterses(PriceCtx mCtx, bool mBuildFromBar){
            m_ctx = mCtx;
            m_build_from_bar = mBuildFromBar;
        }

        private void RemoveLastCluster<T>(SortedDictionary<DateTime, PriceCluster<T>> _items, Session session) where T : class, IDrawable, IPerPriceLevelItem, new(){
            if (0 < _items.Count)
            {
                var _last = _items.Last().Value;
                if (_last.IsMySession(session))
                    _items.Remove(_last.Start);
            }
        }

        public void NewCluster(Session session, TimeSpan _period, TimeSpan ibrPeriod){
            RemoveLastCluster(m_price_profile, session);
            if (0 < session.Data.Length)
                NewClusterImpl(m_price_profile[session.Begin] = new PriceCluster<TPriceProfItem>(m_ctx, session), session, _period, ibrPeriod);
            
            RemoveLastCluster(m_volume_profile, session);
            if (0 < session.Data.Length)
                NewClusterImpl(m_volume_profile[session.Begin] = new PriceCluster<TVolumeProfItem>(m_ctx, session), session, _period, ibrPeriod);
        }

        private void NewClusterImpl<T>(PriceCluster<T> _cluster, Session session, TimeSpan _period, TimeSpan ibrPeriod)
            where T : class, IPerPriceLevelItem, IDrawable, new()
        {
            var tick = 0;
            var _period_num = 0;
            var i = session.Begin;
            var _next = i + _period;
            _next = _next.AddMinutes(-(_next.TimeOfDay.Minutes % _period.Minutes));
            for (; i <= session.End && tick < session.Data.Length; _next = i + _period)
            {
                for (; tick < session.Data.Length; tick++)
                {
                    var _tick = session.Data[tick];
                    if (_tick.Time > _next)
                        break;
                    
                    var item = accept_bar(_cluster, _period_num, _tick);
                    if (tick + 1 == session.Data.Length)
                        item.Last = true;
                }

                if (null == _cluster.IBR && _next - session.Begin >= ibrPeriod && _cluster.Data.Count>0)
                    _cluster.IBR = new KeyValuePair<Price, Price>(_cluster.Data.First().Key, _cluster.Data.Last().Key);

                _period_num++;
                _cluster.NextPeriod();
                i = _next;
            }

            _cluster.PostProcess(_period_num);
        }

        private T accept_bar<T>(PriceCluster<T> _cluster, int _period_num, Bar _tick)
            where T : class, IPerPriceLevelItem, IDrawable, new()
        {
            if (!m_build_from_bar){
                var item = _cluster[_tick.Close];
                item.Hit(_tick.TotalVolume, _period_num);
                return item;
            }

            var low_price = new Price(_tick.Low, m_ctx);
            var _count = (new Price(_tick.High, m_ctx).Int - low_price.Int)/m_ctx.MinMove + 1;
            var _volume_step = _tick.TotalVolume/_count;
            for (var i = 0; i < _count; i++)
                _cluster[(low_price + i).Dbl].Hit(_volume_step, _period_num);
            return _cluster[_tick.Close];
        }

        public void Draw(DrawContext _ctx, DrawSettings _settings)
        {
            
            var _offsets = new Dictionary<DateTime, float>();
            _settings.DrawPriceLines = true;
            if (_settings.VisualSettings.DrawTPO)
                DrawImpl(m_price_profile, _ctx, _settings, _offsets);
            if (_settings.VisualSettings.DrawVolume)
            {
                _settings.DrawPriceLines = false;
                DrawImpl(m_volume_profile, _ctx, _settings, _offsets);
            }
        }

        private void DrawImpl<T>(
            SortedDictionary<DateTime, PriceCluster<T>> _items,
            DrawContext _ctx, DrawSettings _settings, Dictionary<DateTime, float> _offsets)
            where T : class, IDrawable, IPerPriceLevelItem, new()
        {
            var _stable_offsets = new Dictionary<DateTime, float>(_offsets);
            var _env = _ctx.Environment;
            var _start = _env.Point2ChartPoint(_ctx.FullRect.Location).Time;
            var _end = _env.Point2ChartPoint(_ctx.FullRect.Location + _ctx.FullRect.Size).Time;
            var _items_for_draw = _items.Where(_1 => _1.Value.IsMyInterval(_start,_end));
            foreach (var _item in _items_for_draw)
            {
                float _offset = _settings.VisualSettings.Offset2Right;
                if (_stable_offsets.ContainsKey(_item.Key))
                    _offset = _stable_offsets[_item.Key];

                _item.Value.Draw(_ctx, ref _offset, _settings);

                _offsets[_item.Key] = _offset;
            }
        }
    }

    sealed class Session
    {
        public Session(_TPO_ _host, DateTime begin, DateTime end)
        {
            m_host = _host;
            Begin = begin;
            End = end;
        }

        public Bar[] Data
        {
            get { return m_ticks; }
        }

        private readonly _TPO_ m_host;
        
        public DateTime Begin { get; private set; }
        public DateTime End {get; private set; }
        public DateTime LastBar { get; set; }

        private Bar[] m_ticks;
        private volatile bool m_ticks_retrieved;

        public bool DataLoaded { get { return m_ticks_retrieved; } }
        
        public void RequestData(Resolution res)
        {
            m_ticks_retrieved = false;

            var _request = m_host.DataRequest;
            _request.Resolution = res;
            _request.From = Begin;
            _request.To = End;
			_request.RequestStatusDetails = false;

            m_host.Info = string.Format("Loading {2}s for [{0},{1}]...", Begin, End, res.Type.ToString());

            m_host.DataLoader.BeginLoadData(_request, result =>
            {
                m_ticks = result.Data;
                m_ticks_retrieved = result.IsCompleted;
                m_host.DataLoader.EndLoadData(result);

                m_host.Info = string.Format("{3}s [{0},{1}] loaded ({2}).", Begin, End, m_ticks.Length, res.Type.ToString());

                m_host.NewCluster(this);

                m_ticks = null;

                if (m_ticks_retrieved)
                    m_host.SessionComplete();
            }, null);
        }

        public bool OutOfSession(DateTime dateTime){
            return dateTime < Begin || dateTime > End;
        }
    }

    [SameAsSymbol]
    public class _TPO_ : IndicatorObject, IChartCustomDrawer
    {
        private IPlotObjectStr InfoPlot;

        private readonly Timer m_save_timer = new Timer();
        public _TPO_(object _ctx)
            : base(_ctx)
        {
            ProfileType = "Alphas";
            LayerForDraw = EDrawPhases.BeforeSeries;

            m_save_timer.Tick += (_1, _2) => SaveSettings();
            m_save_timer.Interval = 1000;
            m_save_timer.Start();
        }

        protected override void Create()
        {
            ChartCustomDraw.Register(this);
            InfoPlot = AddPlot(new StringPlotAttributes("Info", Color.White));
        }

        private void AddItem2ToolStrip(ToolStrip tb, params ToolStripItem[] item){
            foreach (var stripItem in item)
                stripItem.Tag = this;
            tb.Items.AddRange(item);
        }

        private bool tool_bar_inited;
        void _init_toolbar(){
            if (!tool_bar_inited){
                tool_bar_inited = true;

                ChartToolBar.AccessToolBar(
                    _tb =>
                    {
                        {
                            var _ch_b = new CheckBox { Checked = m_visual_settings.DrawTPO, Text = "Draw TPO" };
                            _ch_b.CheckedChanged += (_1, _2) => { m_visual_settings.DrawTPO = _ch_b.Checked; };
                            AddItem2ToolStrip(_tb, new ToolStripControlHost(_ch_b));
                        }

                        {
                            var _ch_b = new CheckBox { Checked = m_visual_settings.DrawVolume, Text = "Draw Volume" };
                            _ch_b.CheckedChanged += (_1, _2) => { m_visual_settings.DrawVolume = _ch_b.Checked; };
                            AddItem2ToolStrip(_tb, new ToolStripControlHost(_ch_b));
                        }

                        AddItem2ToolStrip(_tb, new ToolStripSeparator());

                        if (ProfileType == c_prof_t_alphas)
                        {
                            AddItem2ToolStrip(_tb,
                                              new ToolStripButton(ColoredShemaStr(m_visual_settings.ColoringShema), null,
                                                                  (_1, _2) =>
                                                                      {
                                                                          var _i = (ToolStripButton) _1;
                                                                          m_visual_settings.ColoringShema =
                                                                              ETPOColoring.ByTime ==
                                                                              m_visual_settings.ColoringShema
                                                                                  ? ETPOColoring.ByPrice
                                                                                  : ETPOColoring.ByTime;
                                                                          _i.Text =
                                                                              ColoredShemaStr(
                                                                                  m_visual_settings.ColoringShema);
                                                                      }));

                            AddItem2ToolStrip(_tb, new ToolStripSeparator());
                        }

                        AddItem2ToolStrip(_tb, m_visual_settings.CreateControls(ProfileType));

                        if (ProfileType == c_prof_t_alphas)
                            AddItem2ToolStrip(_tb,
                                              new ToolStripButton(
                                                  ColoredLettersStr(m_visual_settings.HighlightedLetters), null,
                                                  (_1, _2) =>
                                                      {
                                                          var _i = (ToolStripButton) _1;
                                                          m_visual_settings.HighlightedLetters =
                                                              !m_visual_settings.HighlightedLetters;
                                                          _i.Text =
                                                              ColoredLettersStr(m_visual_settings.HighlightedLetters);
                                                      }));

                        AddItem2ToolStrip(_tb, new ToolStripSeparator());

                        {
                            var _txt_b = new NumericUpDown
                            {
                                Minimum = -500,
                                Maximum = 500,
                                Increment = 10,
                                Value = (decimal)m_visual_settings.Offset2Right
                            };
                            _txt_b.ValueChanged +=
                                (_1, _2) => { m_visual_settings.Offset2Right = (float)((NumericUpDown)_1).Value; };
                            AddItem2ToolStrip(_tb, new ToolStripControlHost(_txt_b));
                        }

                        AddItem2ToolStrip(_tb, new ToolStripSeparator());

                        AddItem2ToolStrip(_tb, new ToolStripButton(m_visual_settings.FontStr, null, (_1, _2) =>
                            {
                                var _i = (ToolStripButton)_1;
                                var _font_dlg = new FontDialog { Font = m_visual_settings.TextFont };
                                if (DialogResult.OK == _font_dlg.ShowDialog())
                                {
                                    m_visual_settings.TextFont = _font_dlg.Font;
                                }
                                _i.Text = m_visual_settings.FontStr;
                            }));
                    });
            }
        }

        private string ColoredShemaStr(ETPOColoring coloringShema){
            return "Colored " + coloringShema.ToString();
        }

        private string ColoredLettersStr(bool val){
            return val ? "Highlighted alphas" : "Colored alphas";
        }

        void _destroy_toolbar(){
            if (tool_bar_inited)
            {
                tool_bar_inited = false;
                ChartToolBar.AccessToolBar(tb =>{
                    var _for_erase = tb.Items.Cast<ToolStripItem>().Where(item => ReferenceEquals(this, item.Tag)).ToList();
                    foreach (var item in _for_erase)
                        tb.Items.Remove(item);
                });
            }
        }

        protected override void StartCalc(){
            
            if (ReferenceEquals(null, m_visual_settings))
                m_visual_settings = VisualSettings.load(this);

            ExecInfo.MaxBarsBack = 0;

            StopCalc();

            m_data_request = Bars.Request;

            lock (this)
            {
                var _ctx = new PriceCtx(new PriceCtx(Bars.Info), TicksRoundUp);
                m_price_clusters = create_cluster(_ctx);
            }

            _init_toolbar();
        }

        protected override void StopCalc(){
            m_last_update = DateTime.MaxValue;
            m_ready_for_draw = false;
            m_sessions_filled = false;
            m_sessions.Clear();
            lock (this)
                m_price_clusters = null;
            _destroy_toolbar();
        }

        [Input]
        public int TicksRoundUp{
            get { return m_TicksRoundUp; }
            set { m_TicksRoundUp = value; }
        }

        private int m_TicksRoundUp = 1;

        internal const string c_prof_t_alphas = "Alphas";
        internal const string c_prof_t_squares = "Squares";

        [Input("Tick", "Minute")]
        public string UnderlyingResolution{
            get { return m_UnderlyingResolution.ToString(); }
            set { m_UnderlyingResolution = (EResolution) Enum.Parse(typeof (EResolution), value); }
        }
        private EResolution m_UnderlyingResolution = EResolution.Tick;

        [Input(c_prof_t_alphas, c_prof_t_squares)]
        public string ProfileType { get; set; }
        private IPriceClusters create_cluster(PriceCtx ctx){
            var _is_tick = build_resolution.IsOneTick();
            switch (ProfileType)
            {
                case c_prof_t_alphas: return new PriceClusterses<AlphasByPrices, VolumeByPrice>(ctx, !_is_tick);
                case c_prof_t_squares: return new PriceClusterses<SquareByPrices, VolumeByPrice>(ctx, !_is_tick);
            }
            throw new ArgumentOutOfRangeException("ProfileType");
        }

        private IPriceClusters m_price_clusters;
        private readonly List<Session> m_sessions = new List<Session>();
        private bool AllTicksRecieved
        {
            get{ return m_sessions.All(session => session.DataLoaded); }
        }

        private volatile bool m_ready_for_draw;
        private volatile bool m_sessions_filled;

        [Input("30", "60", "120", "240", "480", "960")]
        public int TPO_Step_In_Minutes{
            get { return (int)Period.TotalMinutes; }
            set { Period = TimeSpan.FromMinutes(value); }
        }

        [Input("30", "60")]
        public int IBR_Time_In_Minutes{
            get { return (int)IBRPeriod.TotalMinutes; }
            set { IBRPeriod = TimeSpan.FromMinutes(value); }
        }

        public enum ETPOColoring
        {
            ByPrice,
            ByTime
        };

        [Input]
        public EDrawPhases LayerForDraw { get; set; }

        internal TimeSpan Period = TimeSpan.FromMinutes(30);
        internal TimeSpan IBRPeriod = TimeSpan.FromMinutes(30);

        private string m_info;
        internal string Info
        {
            get
            {
                lock (this)
                    return m_info;
            }
            set
            {
                lock (this)
                    m_info = value;
            }
        }

        private DateTime m_last_update;
        protected override void CalcBar()
        {
            fill_sessions();

            if (!m_ready_for_draw && m_sessions_filled && AllTicksRecieved)
            {
                m_ready_for_draw = true;
                m_last_update = DateTime.Now;

                Info = string.Format("Build finished. Last Update {0}", m_last_update);

                ChartCustomDraw.ReDraw();
            }

            InfoPlot.Set(Info);

            if (m_ready_for_draw)
                if (TimeSpan.FromSeconds(30) <= DateTime.Now - m_last_update)
                    SessionComplete();
        }

        private void fill_sessions(){
            if (0 == m_sessions.Count)
            {
                var _sess = Bars.Sessions.SessionForDate(Bars.TimeValue);
                check_session(_sess);
                var _session = new Session(this, _sess.StartOfSession(Bars.TimeValue), _sess.EndOfSession(Bars.TimeValue));
                m_sessions.Add(_session);
            }

            var _last = m_sessions[m_sessions.Count - 1];
            if ( _last.OutOfSession(Bars.TimeValue) ){
                var _sess = Bars.Sessions.SessionForDate(Bars.TimeValue);
                check_session(_sess);
                var _session = new Session(this, _sess.StartOfSession(Bars.TimeValue), _sess.EndOfSession(Bars.TimeValue));
                m_sessions.Add(_session);
                _last = m_sessions[m_sessions.Count - 1];
            }

            _last.LastBar = Bars.TimeValue;

            if (Bars.LastBarOnChart){
                if (!m_sessions_filled){
                    m_sessions_filled = true;
                    Info = string.Format("Sessions calculated ({0}).", m_sessions.Count);
                    SessionComplete();
                }

                if (!AllTicksRecieved || !m_ready_for_draw)
                    ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(1));
            }
        }

        private void check_session(SessionObject _sess){
            if (null == _sess)
                ExecControl.Abort(
                    "Session not found.\nProbably Your select 'Local' timezone for the symbol.\nPlease select 'Exchange' timezone.");
        }

        protected override void OnRecalcLastBarAfterEvent()
        {
            CalcBar();
        }

        protected override void Destroy(){
            _destroy_toolbar();
            ChartCustomDraw.Unregister(this);
            m_save_timer.Stop();
            m_save_timer.Dispose();
        }

        private VisualSettings m_visual_settings;

        public void Draw(DrawContext context, EDrawPhases phase)
        {
            if (LayerForDraw == phase)
            {
                if (ReferenceEquals(null, m_visual_settings))
                    return;

                lock (this)
                {
                    if (null != m_price_clusters){
                        context.DirtyRect = context.FullRect;
                        m_price_clusters.Draw(context, new DrawSettings(m_visual_settings));
                    }
                }
            }
        }

        internal void SessionComplete(){
            foreach (var session in m_sessions.Where(session => !session.DataLoaded))
            {
                session.RequestData(build_resolution);
                return;
            }

            if (!m_ready_for_draw) return;

            var _session = m_sessions.Last();
            if (null == _session || !_session.DataLoaded) return;
            Info = "Update last session ...";
            m_ready_for_draw = false;
            _session.RequestData(build_resolution); // reload last session - for RT
        }

        private Resolution build_resolution{
            get { return new Resolution(m_UnderlyingResolution, 1); }
        }

        private InstrumentDataRequest m_data_request;
        public InstrumentDataRequest DataRequest{
            get { return m_data_request; }
        }

        internal void NewCluster(Session session)
        {
            lock (this)
            {
                m_price_clusters.NewCluster(session, Period, IBRPeriod);
            }
        }

        private volatile bool m_need_save_settings;

        internal void SettingsChanged(){
            ChartCustomDraw.ReDraw();
            m_need_save_settings = true;
        }

        private void SaveSettings(){
            if (m_need_save_settings)
            {
                m_need_save_settings = false;
                m_visual_settings.save();
            }
        }
    }
}