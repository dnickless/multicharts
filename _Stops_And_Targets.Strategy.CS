namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class _Stops_And_Targets : SignalObject
    {
        public _Stops_And_Targets(object ctx) :
            base(ctx)
        {
            StopLossAmt = 1;
            ProfitTargetAmt = 5;
            IsPerContractOption = true;
        }

        [Input]
        public bool IsPerContractOption { get; set; }

        [Input]
        public double ProfitTargetAmt { get; set; }

        [Input]
        public double StopLossAmt { get; set; }

        [Input]
        public double BreakEvenFloorAmt { get; set; }

        [Input]
        public double DollarTrailingAmt { get; set; }

        [Input]
        public double PctTrailingFloorAmt { get; set; }

        [Input]
        public double PctTrailingPct { get; set; }

        [Input]
        public bool IsSetExitOnClose { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = IsPerContractOption ? ESpecOrdersMode.PerContract : ESpecOrdersMode.PerPosition;
            
            if (PublicFunctions.DoubleGreater(ProfitTargetAmt, 0)){
                GenerateProfitTarget(ProfitTargetAmt);
            }
            if (PublicFunctions.DoubleGreater(StopLossAmt, 0)){
                GenerateStopLoss(StopLossAmt);
            }
            if (PublicFunctions.DoubleGreater(BreakEvenFloorAmt, 0)){
                GenerateBreakEven(BreakEvenFloorAmt);
            }
            if (PublicFunctions.DoubleGreater(DollarTrailingAmt, 0)){
                GenerateDollarTrailing(DollarTrailingAmt);
            }
            if (PublicFunctions.DoubleGreater(PctTrailingFloorAmt, 0) 
                &&PublicFunctions.DoubleGreater(PctTrailingPct, 0)){
                GeneratePercentTrailing(PctTrailingFloorAmt, PctTrailingPct);
            }
            if (IsSetExitOnClose){
                GenerateExitOnClose();
            }
        }
    }
}