namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Percent_Trailing : SignalObject
    {
        public Percent_Trailing(object ctx) :
            base(ctx)
        {
            TrailingPct = 20;
            FloorAmt = 1;
        }

        [Input]
        public bool IsPositionBasis { get; set; }

        [Input]
        public double FloorAmt { get; set; }

        [Input]
        public double TrailingPct { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
            GeneratePercentTrailing(FloorAmt, TrailingPct);
        }
    }
}