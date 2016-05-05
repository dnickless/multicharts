using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Volty_Expan_Close_LE : SignalObject
    {
        private IOrderPriced m_VltClsLE;

        public Volty_Expan_Close_LE(object ctx) :
            base(ctx){
            NumAtrs = 0.75;
            Length = 5;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_VltClsLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "VltClsLE", EOrderAction.Buy));
        }

        protected override void CalcBar(){
            m_VltClsLE.Send(Bars.Close[0] + this.AverageTrueRange(Length)*NumAtrs);
        }
    }
}