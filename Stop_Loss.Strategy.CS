namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Stop_Loss : SignalObject
    {
        public Stop_Loss(object ctx) :
            base(ctx)
        {
            Amount = 1;
        }

        [Input]
        public bool IsPositionBasis { get; set; }

        [Input]
        public double Amount { get; set; }


        protected override void CalcBar()
        {
            CurSpecOrdersMode = IsPositionBasis ? ESpecOrdersMode.PerPosition : ESpecOrdersMode.PerContract;
            GenerateStopLoss(Amount);
        }
    }
}