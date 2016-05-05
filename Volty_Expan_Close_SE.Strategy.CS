using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Volty_Expan_Close_SE : SignalObject
    {
        private IOrderPriced m_VltClsSE;

        public Volty_Expan_Close_SE(object ctx) :
            base(ctx){
            NumAtrs = 0.75;
            Length = 5;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_VltClsSE =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "VltClsSE", EOrderAction.SellShort));
        }

        protected override void CalcBar(){
            m_VltClsSE.Send( Bars.Close[0] - this.AverageTrueRange(Length) *NumAtrs);
        }
    }
}