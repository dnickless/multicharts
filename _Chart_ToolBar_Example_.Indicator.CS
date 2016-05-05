using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PowerLanguage.Indicator{
	
    public class _Chart_ToolBar_Example_ : IndicatorObject {
		public _Chart_ToolBar_Example_(object _ctx):base(_ctx){}
		
        protected override void Create(){
            Length = 20;
            m_adx = AddPlot(new PlotAttributes("Avg"));
        }

        private IPlotObject m_adx;

        private int Length { get; set; }

        private void AddItem2ToolStrip(ToolStrip tb, ToolStripItem item){
            item.Tag = this;
            tb.Items.Add(item);
        }

        private bool tool_bar_inited = false;
        protected override void StartCalc()
        {
			if (!tool_bar_inited){
				ChartToolBar.AccessToolBar(tb=>
				                               {
				                                   var _tsi3 = new ToolStripButton {Text = "plot color"};
                                                   set_color_tsi(_tsi3, m_plot_color);
                                                   _tsi3.Click += button1_Click;
                                                   AddItem2ToolStrip(tb, _tsi3);

				                                   var _track = new TrackBar
				                                                    {
				                                                        Dock = DockStyle.Fill,
				                                                        Maximum = 1000,
				                                                        Minimum = 10,
				                                                        SmallChange = 10,
				                                                        Value = Length,
                                                                        Text = "Average Length"                                                                        
				                                                    };
                                                   _track.ValueChanged += _new_track;
                                                   AddItem2ToolStrip(tb, new ToolStripControlHost(_track));

                                                   AddItem2ToolStrip(tb, new ToolStripSeparator());
				                               });
				tool_bar_inited = true;
			}


		}

        private void _new_track(object sender, EventArgs e)
        {
            var _tsi = (TrackBar) sender;
            if (Length != _tsi.Value)
            {
                Length = _tsi.Value;
                ExecControl.Recalculate();
            }
        }

        private Color m_plot_color = Color.Red;

        private static void set_color_tsi(ToolStripButton _tsi, Color _clr){
            var _bmp = new Bitmap(50, _tsi.Height);
            var _gr = Graphics.FromImage(_bmp);
            _gr.FillRectangle(new SolidBrush(_clr), new Rectangle(new Point(0), _bmp.Size));
            _tsi.Image = _bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var _tbi = (ToolStripButton) sender;
            var MyDialog = new ColorDialog {Color = m_plot_color};
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                m_plot_color = MyDialog.Color;
                set_color_tsi(_tbi, MyDialog.Color );
                ExecControl.Recalculate();
            }
        }

        protected override void CalcBar(){
            m_adx.Set( PublicFunctions.Average(Bars.Close, Length), m_plot_color );
		}

        protected override void Destroy(){
            if (tool_bar_inited)
            {
                ChartToolBar.AccessToolBar(tb =>
                                               {
                                                   var _for_erase = new List<ToolStripItem>();
                                                   
                                                   foreach (ToolStripItem item in tb.Items)
                                                       if (ReferenceEquals(this, item.Tag))
                                                           _for_erase.Add(item);
                                                   
                                                   foreach (var item in _for_erase)
                                                       tb.Items.Remove(item);
                                               });
            }
        }
    }
}