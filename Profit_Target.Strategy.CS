namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Profit_Target : SignalObject
    {
        public Profit_Target(object ctx) :
            base(ctx)
        {
            Amount = 5;
        }

        [Input]
        public bool IsPositionBasis { get; set; }

        [Input]
        public double Amount { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
            GenerateProfitTarget(Amount);
        }
    }
}