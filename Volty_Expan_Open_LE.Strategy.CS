using PowerLanguage.Function;

namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
	[CalcAtOpenNextBarAttribute(true)]
    public class Volty_Expan_Open_LE : SignalObject
    {
        private AverageFC 		m_AverageFC;
        private IOrderPriced	m_VltOpenLE;

        public Volty_Expan_Open_LE(object ctx) :
            base(ctx){
            NumRanges = 1.2;
            Length = 4;
        }

        [Input]
        public int Length { get; set; }

        [Input]
        public double NumRanges { get; set; }

        protected override void Create(){
			m_AverageFC = new AverageFC(this);
            m_VltOpenLE = OrderCreator.Stop(new SOrderParameters(Contracts.Default, "VltOpnLE", EOrderAction.Buy));
        }

		protected override void StartCalc(){
            m_AverageFC.price = Bars.RangeSeries();
            m_AverageFC.length = Length;
        }
        protected override void CalcBar(){
            m_VltOpenLE.Send(Bars.OpenNextBar() + m_AverageFC[0]*NumRanges);
        }
    }
}