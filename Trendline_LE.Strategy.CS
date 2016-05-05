using System;
using System.Collections.Generic;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
	[CalcAtOpenNextBarAttribute(true)]
    public class Trendline_LE : SignalObject
    {
        private ITrendLineObject tlRef;

        private DateTime m_BeginTime;

        private IOrderPriced m_BuyOrder;

        public Trendline_LE(object ctx) :
            base(ctx) {}

        [Input]
        public int TlID { get; set; }

        protected override void Create(){
            m_BuyOrder = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Buy", EOrderAction.Buy));
        }

        protected override void StartCalc(){
            IEnumerable<ITrendLineObject> lines = DrwTrendLine.GetTrendLineObjects(EDrawingSource.AnyTechOrManual);
            foreach (ITrendLineObject drw in lines){
                if (drw.ID == TlID){
                    tlRef = drw;
                    m_BeginTime = tlRef.Begin.Time;
                }
            }
        }


        protected override void CalcBar(){
            if (tlRef != null && tlRef.Exist){
                if (Bars.Time[0] >= m_BeginTime && PublicFunctions.DoubleLess(Bars.High[0], tlRef.PriceValue(Bars.Time[0])))
                {
                    m_BuyOrder.Send(tlRef.PriceValue(Bars.TimeNextBar()));
                }
            }
        }
    }
}