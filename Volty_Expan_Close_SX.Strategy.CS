using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Volty_Expan_Close_SX : SignalObject
    {
        private IOrderPriced m_VltClsSX;

        public Volty_Expan_Close_SX(object ctx) :
            base(ctx){
            NumAtrs = 1.5;
            Length = 5;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_VltClsSX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "VltClsSX", EOrderAction.BuyToCover,
                                                             OrderExit.FromAll));
        }

        protected override void CalcBar(){
            m_VltClsSX.Send(Bars.Close[0] + this.AverageTrueRange(Length)*NumAtrs);
        }
    }
}