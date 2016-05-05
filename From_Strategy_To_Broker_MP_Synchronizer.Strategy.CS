using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Enabled)]
    public class From_Strategy_To_Broker_MP_Synchronizer :
        SignalObject
    {
        private double m_timeoutms = 1000;

        private double m_latencyms = 1000;

        private ITextObject m_textid;

        private string m_sync_state;

        private string m_diff_state;

        private string m_correction_state;

        private Boolean m_mp_diff;

        private Double m_mp_diff_time_start;

        private Boolean m_mp_corrected;

        private Double m_mp_corrected_time_start;

        private Boolean m_place_correction_marketorder;

       
   
       
        public From_Strategy_To_Broker_MP_Synchronizer(object ctx) :
            base(ctx) {}

        [Input]
        public double TimeOut_ms{
            get { return m_timeoutms; }
            set { m_timeoutms = value; }
        }

        [Input]
        public double Latency_ms{
            get { return m_latencyms; }
            set { m_latencyms = value; }
        }

        protected override void StartCalc(){
            m_textid = DrwText.Create(new ChartPoint(Bars.Time[0], Bars.Close[0]), "CurrentState");
            m_sync_state = "MP is synchronized!";
            m_diff_state = "MP syncronization. Wait " + (TimeOut_ms*.001) + " seconds.";
            m_correction_state = "MP correction order sent. Wait " + (Latency_ms*0.001) + " seconds.";
            m_mp_diff = false;
            m_mp_diff_time_start = 0;
            m_mp_corrected = false;
            m_mp_corrected_time_start = 0;
            m_place_correction_marketorder = false;
        }


        protected override void CalcBar(){
            if (Environment.IsRealTimeCalc&&Environment.IsAutoTradingMode){
                var m_inner_mp = StrategyInfo.MarketPosition;
                var m_broker_mp = StrategyInfo.MarketPositionAtBroker;
                var m__rightest = Environment.RightScreenTime;
                var m__highest = Environment.HighestScaleValue;
                m_textid.Location = new ChartPoint(m__rightest, m__highest);
                if (m_broker_mp != m_inner_mp){
                    var m__get_tick_count = DateTime.Now.Ticks;
                    if (!m_mp_diff && !m_mp_corrected){
                        m_mp_diff = true;
                        m_mp_diff_time_start = m__get_tick_count;
                        m_textid.Text = m_diff_state;
                    }
                    if (m_mp_diff && !m_mp_corrected
                         && PublicFunctions.DoubleGreater(m__get_tick_count - m_mp_diff_time_start, TimeOut_ms))
                    {
                        m_place_correction_marketorder = true;
                        m_mp_corrected = true;
                        m_mp_corrected_time_start = m__get_tick_count;
                    }
                    if (m_mp_corrected){
                        m_textid.Text = m_correction_state;
                        if (PublicFunctions.DoubleGreater(m__get_tick_count - m_mp_corrected_time_start,
                                                          Latency_ms)){
                            m_mp_corrected_time_start = m__get_tick_count;
                            m_mp_diff = false;
                            m_mp_corrected = false;
                        }
                    }
                    if (m_place_correction_marketorder){
                        int m_correct_contracts = Math.Abs(m_broker_mp - m_inner_mp);
                        m_place_correction_marketorder = false;
                        bool m_is_buy = PublicFunctions.IffLogic(m_broker_mp > m_inner_mp,
                                                            false, true);
                        GenerateATMarketOrder(m_is_buy, true, m_correct_contracts);
                    }
                }
                else{
                    m_textid.Text = m_sync_state;
                    m_mp_corrected = false;
                    m_mp_diff = false;
                }
            }
        }
    }
}