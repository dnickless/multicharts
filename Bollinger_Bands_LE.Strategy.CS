using System;

namespace PowerLanguage.Strategy
{
    public class Bollinger_Bands_LE : SignalObject
    {
        private IOrderPriced m_BBandLE;

        public Bollinger_Bands_LE(object ctx) :
            base(ctx)
        {
            Length = 20;
            NumDevsDn = 2;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumDevsDn { get; set; }

        private VariableSeries<double> m_LowerBand;

        protected override void Create()
        {
            m_LowerBand = new VariableSeries<Double>(this);
            m_BBandLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "BBandLE", EOrderAction.Buy));
        }

        protected override void CalcBar()
        {
            m_LowerBand.Value = Bars.Close.BollingerBandCustom(Length, -NumDevsDn);
            if (Bars.CurrentBar > 1 && Bars.Close.CrossesOver(m_LowerBand, ExecInfo.MaxBarsBack))
                m_BBandLE.Send(m_LowerBand.Value);
        }
    }
}