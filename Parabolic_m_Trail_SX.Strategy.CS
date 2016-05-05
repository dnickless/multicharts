using System;
using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Parabolic_m_Trail_SX : SignalObject
    {
        private VariableSeries<int> m_MP;

        private VariableObject<Double> m_StopPrice;

        private VariableObject<Double> m_AF;

        private VariableSeries<Double> m_TradeHH;

        private IOrderPriced m_ParTrSX;

        public Parabolic_m_Trail_SX(object ctx) :
            base(ctx){
            NumAtrs = 1.5;
            AtrLength = 3;
            AccFactorLimit = 0.2;
            AccFactorStep = 0.02;
        }

        [Input]
        public double AccFactorStep { get; set; }

        [Input]
        public double AccFactorLimit { get; set; }

        [Input]
        public int AtrLength { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_MP = new VariableSeries<int>(this);
            m_TradeHH = new VariableSeries<Double>(this);
            m_ParTrSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "ParTrSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
            m_StopPrice = new VariableObject<double>(this);
            m_AF = new VariableObject<double>(this);
        }

        

        protected override void CalcBar(){
            m_MP.Value = StrategyInfo.MarketPosition;
            if (m_MP.Value < 0)
            {
                if (m_MP[1] >= 0)
                {
                    m_StopPrice.Value = Bars.High[0] + this.AverageTrueRange(AtrLength)*NumAtrs;
                    m_AF.Value = AccFactorStep;
                    m_TradeHH.Value = Bars.Low[0];
                }
                else{
                    if (PublicFunctions.DoubleLess(Bars.Low[0], m_TradeHH.Value)){
                        m_TradeHH.Value = Bars.Low[0];
                    }
                    m_StopPrice.Value -= m_AF.Value * (m_StopPrice.Value - m_TradeHH.Value);

                    if (PublicFunctions.DoubleLess(m_TradeHH.Value, m_TradeHH[1])
                        && PublicFunctions.DoubleLess(m_AF.Value, AccFactorLimit))
                        m_AF.Value += PublicFunctions.Min(AccFactorStep, AccFactorLimit - m_AF.Value);
                }
                if (PublicFunctions.DoubleLess(m_StopPrice.Value, Bars.High[0]))
                    m_StopPrice.Value = Bars.High[0];
                m_ParTrSX.Send(m_StopPrice.Value);
            }
        }
    }
}