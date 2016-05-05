namespace PowerLanguage.Strategy
{
    public class _Percent_Exit_Pair : SignalObject
    {
        public _Percent_Exit_Pair(object ctx) :
            base(ctx)
        {
            StopLossPct = 0.05;
            ProfitTargetPct = 0.1;
        }

        [Input]
        public double ProfitTargetPct { get; set; }

        [Input]
        public double StopLossPct { get; set; }


        protected override void CalcBar(){
            CurSpecOrdersMode = ESpecOrdersMode.PerContract;
            if (StrategyInfo.MarketPosition != 0){
                if (PublicFunctions.DoubleGreater(ProfitTargetPct, 0)){
                    GenerateProfitTarget(this.EntryPrice()*ProfitTargetPct);
                }
                if (PublicFunctions.DoubleGreater(StopLossPct, 0)){
                    GenerateStopLoss(this.EntryPrice()*StopLossPct);
                }
            }
            else{
                if (PublicFunctions.DoubleGreater(ProfitTargetPct, 0)){
                    GenerateProfitTarget(Bars.Close[0]*ProfitTargetPct);
                }
                if (PublicFunctions.DoubleGreater(StopLossPct, 0)){
                    GenerateStopLoss(Bars.Close[0]*StopLossPct);
                }
            }
        }
    }
}