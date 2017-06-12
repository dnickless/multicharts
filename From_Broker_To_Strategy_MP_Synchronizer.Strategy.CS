using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Enabled)]
    public class From_Broker_To_Strategy_MP_Synchronizer : SignalObject
    {
        private ITextObject m_textid;

        private string m_sync_state;
        private string m_diff_state;
        private DateTime m_mp_diff_detected_time;

        private bool m_mp_diff;
        private bool m_mp_corrected;

        private DateTime m_mp_diff_time_start;
        private DateTime m_mp_corrected_time_start;

        public From_Broker_To_Strategy_MP_Synchronizer(object ctx) :
            base(ctx){
            LatencyMS = 500;
            TimeOutMS = 1000;
        }

        [Input]
        public int TimeOutMS { get; set; }

        [Input]
        public int LatencyMS { get; set; }

        protected override void StartCalc(){
            m_textid = Environment.ApplicationCode == EApplicationCode.Portfolio ? null : DrwText.Create(new ChartPoint(Bars.Time[0], Bars.Close[0]), "CurrentState");
			if (m_textid != null) {
	            m_textid.HStyle = ETextStyleH.Left;
	            m_textid.VStyle = ETextStyleV.Below;
			}
            m_sync_state = "MP is synchronized!";
            m_diff_state = string.Format("MP syncronization.\nWait {0} seconds", TimeOutMS*0.001);
            m_mp_diff = false;
            m_mp_corrected = false;
        }

        protected override void Destroy() {}

        protected override void CalcBar(){
            if (!Environment.IsRealTimeCalc || !Environment.IsAutoTradingMode)
                return;
            {
                var m_inner_mp = StrategyInfo.MarketPosition;
                var m_broker_mp = StrategyInfo.MarketPositionAtBroker;

				if (m_textid != null)
					m_textid.Location = new ChartPoint(Environment.RightScreenTime, Environment.HighestScaleValue);

                if (m_broker_mp != m_inner_mp){
                    m_mp_diff_detected_time = DateTime.Now;

                    if (!m_mp_diff && !m_mp_corrected){
                        m_mp_diff = true;
                        m_mp_diff_time_start = m_mp_diff_detected_time;
						if (m_textid != null)
							m_textid.Text = m_diff_state;
                    }

                    var _exit_price = .0;
                    var _place_order = false;

                    if (m_mp_diff && !m_mp_corrected){
                        _exit_price = Bars.Close[0];
                        if ((m_mp_diff_detected_time - m_mp_diff_time_start).TotalMilliseconds >= TimeOutMS){
                            _place_order = true;
                            m_mp_corrected = true;
                            m_mp_corrected_time_start = m_mp_diff_detected_time;
                        }
                    }

                    if (m_mp_corrected){
                        if ((m_mp_diff_detected_time - m_mp_corrected_time_start).TotalMilliseconds >= LatencyMS){
                            m_mp_corrected_time_start = m_mp_diff_detected_time;
                            m_mp_diff = false;
                            m_mp_corrected = false;
                        }
                    }

                    if (_place_order){
                        if (0 != m_broker_mp)
                            _exit_price = StrategyInfo.AvgEntryPriceAtBroker;
                        ChangeMarketPosition(m_broker_mp - m_inner_mp, _exit_price, "Sync Order");
                    }
                }
                else{
					if (m_textid != null)
						m_textid.Text = m_sync_state;
                    m_mp_corrected = false;
                    m_mp_diff = false;
                }
            }
        }
    }
}