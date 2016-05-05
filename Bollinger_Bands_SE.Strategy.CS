using System;

namespace PowerLanguage.Strategy
{
    public class Bollinger_Bands_SE : SignalObject
    {
        private IOrderPriced m_BBandSE;

        public Bollinger_Bands_SE(object ctx) :
            base(ctx)
        {
            NumDevsUp = 2;
            Length = 20;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumDevsUp { get; set; }

        private VariableSeries<double> m_HigherBand;

        protected override void Create()
        {
            m_HigherBand = new VariableSeries<Double>(this);
            m_BBandSE =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "BBandSE", EOrderAction.SellShort));
        }

        protected override void CalcBar()
        {
            m_HigherBand.Value = Bars.Close.BollingerBandCustom(Length, NumDevsUp);
            if (Bars.CurrentBar > 1 && Bars.Close.CrossesUnder(m_HigherBand, ExecInfo.MaxBarsBack))
                m_BBandSE.Send(m_HigherBand.Value);
        }
    }
}