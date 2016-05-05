using System;
using System.Collections.Generic;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
	[CalcAtOpenNextBarAttribute(true)]
    public class Trendline_SE : SignalObject
    {
        private ITrendLineObject tlref;

        private DateTime m_BeginTime;

       
        private IOrderPriced m_ShortOrder;

        public Trendline_SE(object ctx) :
            base(ctx) {}

        [Input]
        public int tlID { get; set; }

        protected override void Create(){
            m_ShortOrder = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "Short", EOrderAction.SellShort));
        }

        protected override void StartCalc(){
            IEnumerable<ITrendLineObject> lines = DrwTrendLine.GetTrendLineObjects(EDrawingSource.AnyTechOrManual);
            foreach (ITrendLineObject drw in lines){
                if (drw.ID == tlID){
                    tlref = drw;
                    m_BeginTime = tlref.Begin.Time;
                }
            }
        }


        protected override void CalcBar(){
            if (tlref != null && tlref.Exist){
                if (Bars.Time[0] >= m_BeginTime 
                    && PublicFunctions.DoubleGreater(Bars.Low[0], tlref.PriceValue(Bars.Time[0])))
                {
                    m_ShortOrder.Send(tlref.PriceValue(Bars.TimeNextBar()));
                }
            }
        }
    }
}