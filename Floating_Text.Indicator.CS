using System;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    [RecoverDrawings(false)]
    public class Floating_Text : IndicatorObject
    {
        private ITextObject m_text_obj;

        public Floating_Text(object ctx) :
            base(ctx){
            textpricepercent = 50;
            textbarsback = 10;
            texttoplot = "MyText";
        }

        [Input]
        public string texttoplot { get; set; }

        [Input]
        public int textbarsback { get; set; }

        [Input]
        public double textpricepercent { get; set; }

        protected override void CalcBar(){
            if (!Bars.LastBarOnChart) return;
            if (m_text_obj == null)
            {
                m_text_obj = DrwText.Create(new ChartPoint(Bars.Time[0], Bars.Close[0]), texttoplot);
            }
            else{
                this.Text_Float(m_text_obj, textbarsback, textpricepercent);
            }
        }
    }
}