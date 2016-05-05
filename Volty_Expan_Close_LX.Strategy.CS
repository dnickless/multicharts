using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Volty_Expan_Close_LX : SignalObject
    {
        private IOrderPriced m_VltClsLX;

        public Volty_Expan_Close_LX(object ctx) :
            base(ctx){
            NumAtrs = 1.5;
            Length = 5;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumAtrs { get; set; }

        protected override void Create(){
            m_VltClsLX =
                OrderCreator.Stop(new SOrderParameters(Contracts.Default, "VltClsLX", EOrderAction.Sell,
                                                             OrderExit.FromAll));
        }

        protected override void CalcBar(){
            m_VltClsLX.Send(Bars.Close[0]-this.AverageTrueRange(Length) * NumAtrs);
        }
    }
}