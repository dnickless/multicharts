namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Breakeven_Stop : SignalObject
    {
        public Breakeven_Stop(object ctx) :
            base(ctx)
        {
            FloorAmt = 1;
        }

        [Input]
        public bool IsPositionBasis { get; set; }

        [Input]
        public double FloorAmt { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
            GenerateBreakEven(FloorAmt);
        }
    }
}