using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    [RecoverDrawings(false)]
    public class _Market_Depth_on_Chart_ : IndicatorObject
    {
        private draw_DOM_level m_draw_dom_level1;
        private draw_DOM_level m_draw_dom_level2;

        private double m_value11;

        private IPlotObjectStr Plot1;

        public _Market_Depth_on_Chart_(object ctx) :
            base(ctx){
            updatespeed_seconds = 0.1;
        }

        [Input]
        public double updatespeed_seconds { get; set; }

        protected override void Create(){
            m_draw_dom_level1 = new draw_DOM_level(this);
            m_draw_dom_level2 = new draw_DOM_level(this);
            Plot1 = AddPlot(new StringPlotAttributes("Status", Color.Red));
        }

        protected override void StartCalc(){
            m_draw_dom_level1.Data = new Lambda<DOMPrice[]>(back => Bars.DOM.Ask);
            m_draw_dom_level1.maxsize = new Lambda<Double>(_bb => m_value11);
            m_draw_dom_level1.From = Color.FromArgb(255, 0, 0);
            m_draw_dom_level1.To = Color.FromArgb(80, 0, 0);

            m_draw_dom_level2.Data = new Lambda<DOMPrice[]>(back => Bars.DOM.Bid);
            m_draw_dom_level2.maxsize = new Lambda<Double>(_bb => m_value11);
            m_draw_dom_level2.From = Color.FromArgb(0, 0, 255);
            m_draw_dom_level2.To = Color.FromArgb(0, 0, 80);
        }

        protected override void CalcBar(){
            if (!Bars.LastBarOnChart)
                return;

            ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(updatespeed_seconds));

            if (Bars.DOM.Connected){
                m_value11 = 0;

                foreach (var domPrice in Bars.DOM.Ask)
                    m_value11 = Math.Max(m_value11, domPrice.Size);
                foreach (var domPrice in Bars.DOM.Bid)
                    m_value11 = Math.Max(m_value11, domPrice.Size);

				if (m_value11 == 0) return;
                m_draw_dom_level1.Call();
                m_draw_dom_level2.Call();

                Plot1.Set("OK", Color.White);
            }
            else{
                Plot1.Set("Level2 data is not avaliable", Color.Red);
            }
        }
    }
}