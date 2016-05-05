using System;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class TimeExit_Calndr_LX : SignalObject
    {
        private TimeSpan m_Intervall;


        private IOrderMarket m_TimeCalLX;

        public TimeExit_Calndr_LX(object ctx) :
            base(ctx)
        {
            NumDays = 3;
        }

        [Input]
        public int NumDays { get; set; }

        [Input]
        public int NumHours { get; set; }

        [Input]
        public int NumMinutes { get; set; }

        protected override void Create(){
            m_TimeCalLX =
                OrderCreator.MarketNextBar(new SOrderParameters(Contracts.Default, "TimeCalLX", EOrderAction.Sell,
                                                                      OrderExit.FromAll));
        }

        protected override void StartCalc(){
            m_Intervall = new TimeSpan(NumDays, NumHours, NumMinutes, 0);
        }


        protected override void CalcBar(){
            int m_MP = StrategyInfo.MarketPosition;

            if (m_MP != 0){
                DateTime TargetTime = this.EntryTime() + m_Intervall;
                if (TargetTime <= Bars.Time[0]){
                    if (m_MP > 0){
                        m_TimeCalLX.Send();
                    }
                }
            }
        }
    }
}