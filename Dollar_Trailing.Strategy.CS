namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Dollar_Trailing : SignalObject
    {
        public Dollar_Trailing(object ctx) :
            base(ctx)
        {
            Amount = 1;
        }

        [Input]
        public bool IsPositionBasis { get; set; }

        [Input]
        public double Amount { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
            GenerateDollarTrailing(Amount);
        }
    }
}