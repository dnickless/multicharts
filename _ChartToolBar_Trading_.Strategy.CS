using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ATCenterProxy.interop;
using PowerLanguage.TradeManager;

namespace PowerLanguage.Strategy
{
    public class _ChartToolBar_Trading_ : SignalObject
    {
        public _ChartToolBar_Trading_(object _ctx) : base(_ctx)
        {
        }

        private void AddItem2ToolStrip(ToolStrip tb, ToolStripItem item)
        {
            item.Tag = this;
            tb.Items.Add(item);
        }

        private ToolStripButton m_info_panel;
        private bool tool_bar_inited;

        protected override void StartCalc()
        {
            if (!tool_bar_inited)
            {
                tool_bar_inited = true;
                ChartToolBar.AccessToolBar(tb =>
                                               {
                                                   var _tsi_b = new ToolStripButton
                                                                    {
                                                                        Text = "Buy Market",
                                                                        BackColor = Color.DeepSkyBlue,
                                                                        ToolTipText = "Click for send Buy 1 Market"
                                                                    };
                                                   _tsi_b.Click += (_1, _2) => _tsi_b_Click(MTPA_OrdrActn.eMTPA_OA_Buy);
                                                   AddItem2ToolStrip(tb, _tsi_b);

                                                   m_info_panel = new ToolStripButton
                                                                      {ToolTipText = "Click for Close Position"};
                                                   m_info_panel.Click += _info_panel_Click;
                                                   set_panel_info(0, 0);
                                                   AddItem2ToolStrip(tb, m_info_panel);

                                                   var _tsi_s = new ToolStripButton
                                                                    {
                                                                        Text = "Sell Market",
                                                                        BackColor = Color.LightCoral,
                                                                        ToolTipText = "Click for send Sell 1 Market"
                                                                    };
                                                   _tsi_s.Click += (_1, _2) => _tsi_b_Click(MTPA_OrdrActn.eMTPA_OA_Sell);
                                                   AddItem2ToolStrip(tb, _tsi_s);

                                                   AddItem2ToolStrip(tb, new ToolStripSeparator());
                                               });
            }


            m_prof = null;
            m_symbol = new MCSymbolInfo(Bars.Info.ASymbolInfo2, Bars.Info.DataFeed);

            var _dummy = Prof;
        }

        private void _info_panel_Click(object sender, EventArgs ev)
        {
            try
            {
                if (0 != m_cur_mp)
                    Prof.PlaceOrder(
                        new OrderParams
                            {
                                action = 0 < m_cur_mp ? MTPA_OrdrActn.eMTPA_OA_Sell : MTPA_OrdrActn.eMTPA_OA_Buy,
                                category = MTPA_OrdrCtgry.eMTPA_OC_Market,
                                contracts = Math.Abs(m_cur_mp),
                                tif = MTPA_OrdrTimeInForce.eMTPA_TIF_GTC
                            });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private volatile int m_cur_mp;

        private void set_panel_info(int mp, double opl)
        {
            m_cur_mp = mp;
            ChartToolBar.AccessToolBarAsync(tb =>
                                                {
                                                    m_info_panel.Enabled = 0 != mp;
                                                    m_info_panel.Text = MakeInfoPanelStr(mp, opl);
                                                    m_info_panel.BackColor = GetColor(opl);
                                                });
        }

        private static string MakeInfoPanelStr(int mp, double opl)
        {
            string _mp_string;
            if (0 < mp)
                _mp_string = mp.ToString() + " Long";
            else if (0 > mp)
                _mp_string = (-mp).ToString() + " Short";
            else
                _mp_string = "Flat";
            return string.Format("{0} {1}", _mp_string, opl.ToString("C"));
        }

        private static Color GetColor(double opl)
        {
            return 0 > opl ? Color.OrangeRed : Color.LawnGreen;
        }

        private MCSymbolInfo m_symbol;

        private ITradingProfile m_prof;

        public ITradingProfile Prof
        {
            get
            {
                if (null == m_prof)
                    foreach (var profile in TradeManager.TradingProfiles)
                        if (profile.Name == Profile)
                        {
                            if (ETM_ConnectionChanged.eTM_CC_Connected != profile.ConnectionState)
                                ExecControl.Abort("Profile {0} is not connected!", profile.Name);

                            m_prof = profile;

                            var _acc = m_prof.Accounts[0];
                            m_prof.CurrentAccount = _acc;
                            m_prof.CurrentSymbol = m_symbol;

                            var _poss = TradeManager.TradingData.Positions;
                            _poss.AccountFltr.CurrentValue = _acc;
                            _poss.SymbolFltr.CurrentValue = m_symbol.symbol.SymbolName;
                            _poss.ProfileFltr.CurrentValue = Profile;

                            _poss.Added += _poss_changed;
                            _poss.Changed += _poss_changed;
                            _poss.Deleted += _poss_changed;

                            break;
                        }
                return m_prof;
            }
        }

        private void _poss_changed(Position[] _dummy)
        {
            var _poss = TradeManager.TradingData.Positions.Items;
            if (0 == _poss.Length)
                set_panel_info(0, 0);
            else
                set_panel_info(_poss[0].Value, _poss[0].OpenPL);
        }

        private void _tsi_b_Click(MTPA_OrdrActn act)
        {
            try
            {
                Prof.PlaceOrder(
                    new OrderParams
                        {
                            action = act,
                            category = MTPA_OrdrCtgry.eMTPA_OC_Market,
                            contracts = 1,
                            tif = MTPA_OrdrTimeInForce.eMTPA_TIF_GTC
                        });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        protected override void CalcBar()
        {
            if (Bars.LastBarOnChart)
                ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(0.5));
        }

        protected override void OnRecalcLastBarAfterEvent()
        {
            TradeManager.ProcessEvents();
            ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(0.5));
        }

        protected override void Destroy()
        {
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