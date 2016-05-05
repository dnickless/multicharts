namespace PowerLanguage.Strategy
{
    [IOGMode(IOGMode.Disabled)]
    public class Close_at_End_of_Day : SignalObject
    {
        public Close_at_End_of_Day(object ctx) :
            base(ctx) {}

        protected override void CalcBar(){
            GenerateExitOnClose();
        }
    }
}