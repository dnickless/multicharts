namespace PowerLanguage.Indicator
{
    [RecoverDrawings(false)]
    [MouseEvents]
    public class MouseEventsExample : IndicatorObject
    {
        public MouseEventsExample(object _ctx) : base(_ctx) { }
        
        protected override void CalcBar(){}

        private ITextObject m_text;
        protected override void OnMouseEvent(MouseClickArgs arg){
            if (null != m_text)
                m_text.Delete();
            m_text = DrwText.Create(
                arg.point, string.Format("user mouse {0} click at bar {1} on data stream {2}",
                arg.buttons, arg.bar_number, arg.data_number));
        }
    }
}