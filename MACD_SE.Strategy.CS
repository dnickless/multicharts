using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    public class MACD_SE : SignalObject
    {
        private MACD m_MACD;

        private VariableSeries<Double> m_MACD_diff;
        private VariableSeries<Double> m_my_MACD;

        private IOrderMarket m_MacdSE;
        private XAverage m_XAverage;

        public MACD_SE(object ctx) :
            base(ctx)
        {
            FastLength = 12;
            SlowLength = 26;
            MacdLength = 9;
        }

        [Input]
        public int FastLength { get; set; }

        [Input]
        public int SlowLength { get; set; }

        [Input]
        public int MacdLength { get; set; }

        protected override void Create()
        {
            m_MACD = new MACD(this);
            m_XAverage = new XAverage(this);
            m_my_MACD = new VariableSeries<Double>(this);
            m_MACD_diff = new VariableSeries<Double>(this);
            m_MacdSE =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "MacdSE",
                                                                EOrderAction.SellShort));
        }

        protected override void StartCalc()
        {
            m_MACD.Price = Bars.Close;
            m_MACD.FastLength = FastLength;
            m_MACD.SlowLength = SlowLength;
            m_XAverage.Price = m_my_MACD;
            m_XAverage.Length = MacdLength;
        }


        protected override void CalcBar()
        {
            m_my_MACD.Value = m_MACD[0];
            double MACD_avg = m_XAverage[0];
            m_MACD_diff.Value = m_my_MACD.Value - MACD_avg;
            if (Bars.CurrentBar > 2 && m_MACD_diff.CrossesUnder(0, ExecInfo.MaxBarsBack))
                m_MacdSE.Send();
        }
    }
}