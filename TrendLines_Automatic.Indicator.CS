using System;
using System.Drawing;
using PowerLanguage.Function;

namespace PowerLanguage.Indicator
{
    [SameAsSymbol(true)]
    public class TrendLines_Automatic : IndicatorObject
    {
        private BarNumber m_barnumber1;

        private SwingHighBar m_swinghighbar1;

        private SwingLowBar m_swinglowbar1;

        private VariableSeries<ITrendLineObject> m_dntlref;

        private VariableObject<Int32> m_dntlendbar;

        private VariableObject<Boolean> m_dntlbreak;

        private VariableSeries<ITrendLineObject> m_uptlref;

        private VariableObject<Int32> m_uptlendbar;

        private VariableObject<Boolean> m_uptlbreak;

        private VariableObject<Int32> m_index;

        private bool m_historytf;

        private string m_alerttypecaps;

        private ArraySimple<DateTime> m_swhitime;

        private ArraySimple<Double> m_swhival;

        private ArraySimple<DateTime> m_swlotime;

        private ArraySimple<Double> m_swloval;

        public TrendLines_Automatic(object ctx) :
            base(ctx){
            alerttype = "IntraBar";
            uptlcolor = Color.Cyan;
            dntlcolor = Color.Red;
            history = "Yes";
            barspast = 10;
            swlostrength = 4;
            swhistrength = 4;
        }

        [Input]
        public int swhistrength { get; set; }

        [Input]
        public int swlostrength { get; set; }

        [Input]
        public int barspast { get; set; }

        [Input]
        public string history { get; set; }

        [Input]
        public Color dntlcolor { get; set; }

        [Input]
        public Color uptlcolor { get; set; }

        [Input]
        public string alerttype { get; set; }

        protected override void Create(){
            m_barnumber1 = new BarNumber(this);
            m_swinghighbar1 = new SwingHighBar(this);
            m_swinglowbar1 = new SwingLowBar(this);
            m_dntlref = new VariableSeries<ITrendLineObject>(this);
            m_dntlendbar = new VariableObject<Int32>(this);
            m_dntlbreak = new VariableObject<Boolean>(this);
            m_uptlref = new VariableSeries<ITrendLineObject>(this);
            m_uptlendbar = new VariableObject<Int32>(this);
            m_uptlbreak = new VariableObject<Boolean>(this);
            m_index = new VariableObject<Int32>(this);
            m_swhitime = new ArraySimple<DateTime>(this, 11);
            m_swhival = new ArraySimple<Double>(this, 11);
            m_swlotime = new ArraySimple<DateTime>(this, 11);
            m_swloval = new ArraySimple<Double>(this, 11);
        }

        protected override void StartCalc(){
            m_swinghighbar1.instance = 1;
            m_swinghighbar1.price = Bars.High;
            m_swinghighbar1.strength = swhistrength;
            m_swinghighbar1.length = swhistrength + 1;
            m_swinglowbar1.instance = 1;
            m_swinglowbar1.price = Bars.Low;
            m_swinglowbar1.strength = swlostrength;
            m_swinglowbar1.length = swlostrength + 1;
            m_dntlendbar.DefaultValue = 0;
            m_dntlbreak.DefaultValue = false;
            m_uptlendbar.DefaultValue = 0;
            m_uptlbreak.DefaultValue = false;
            m_index.DefaultValue = 0;
            m_historytf = (history.ToUpper() == "YES") || (history.ToUpper() == "Y");
            m_alerttypecaps = alerttype.ToUpper();
            m_swhival.DefaultValue = (-1*1000000);
            m_swloval.DefaultValue = 1000000;
        }


        protected override void CalcBar(){
            var m_barnum = m_barnumber1[0];
            if ((m_swinghighbar1[0] == swhistrength)){
                for (var m_value1 = 9; m_value1 >= 0; m_value1 = (m_value1 - 1)){
                    m_swhitime[0][(((m_value1 + 1)))] = m_swhitime[0][((m_value1))];
                    m_swhival[0][(((m_value1 + 1)))] = m_swhival[0][((m_value1))];
                }
                m_swhitime[0][0] = Bars.Time[swhistrength];
                m_swhival[0][0] = Bars.High[swhistrength];
                var m_value2 = 1;
                for (; m_value2 <= 10; m_value2 = (m_value2 + 1))
                {
                    if ((m_swhival[0][((m_value2))] > m_swhival[0][0]))
                    {
                        m_index.Value = m_value2;
                        m_value2 = 11;
                    }
                }
                if ((m_value2 == 12)){
                    if (m_dntlref[0] != null && m_dntlref[0].Exist){
                        if ((m_historytf && (m_dntlbreak.Value == false))){
                            m_dntlref.Value.End = new ChartPoint(Bars.Time[0],
                                                                 m_dntlref.Value.PriceValue(Bars.Time[0]));
                            m_dntlref.Value.ExtRight = false;
                        }
                        else{
                            if (!m_historytf){
                                m_dntlref.Value.Delete();
                            }
                        }
                    }
                    m_dntlref.Value =
                        DrwTrendLine.Create(
                            new ChartPoint(m_swhitime[0][((m_index.Value))], m_swhival[0][((m_index.Value))]),
                            new ChartPoint(m_swhitime[0][0], m_swhival[0][0]));
                    if (m_dntlbreak.Value){
                        m_dntlbreak.Value = false;
                    }
                    m_dntlendbar.Value = (m_barnum - swhistrength);
                    m_dntlref.Value.ExtLeft = false;
                    m_dntlref.Value.ExtRight = true;
                    m_dntlref.Value.Color = dntlcolor;
                    if ((m_alerttypecaps == "ONCLOSE")){
                        m_dntlref.Value.Alert = EAlertType.BreakoutOnClose;
                    }
                    else{
                        if ((m_alerttypecaps == "INTRABAR")){
                            m_dntlref.Value.Alert = EAlertType.BreakoutIntrabar;
                        }
                        else{
                            m_dntlref.Value.Alert = EAlertType.Disabled;
                        }
                    }
                }
            }
            if ((m_swinglowbar1[0] == swlostrength)){
                for (var m_value1 = 9; m_value1 >= 0; m_value1 = (m_value1 - 1)){
                    m_swlotime[0][(((m_value1 + 1)))] = m_swlotime[0][((m_value1))];
                    m_swloval[0][(((m_value1 + 1)))] = m_swloval[0][((m_value1))];
                }
                m_swlotime[0][0] = Bars.Time[swlostrength];
                m_swloval[0][0] = Bars.Low[swlostrength];
                var m_value2 = 1;
                for (; m_value2 <= 10; m_value2 = (m_value2 + 1))
                {
                    if ((m_swloval[0][((m_value2))] < m_swloval[0][0]))
                    {
                        m_index.Value = m_value2;
                        m_value2 = 11;
                    }
                }
                if ((m_value2 == 12)){
                    if (m_uptlref.Value != null && m_uptlref.Value.Exist){
                        if (m_historytf && !m_uptlbreak.Value){
                            m_uptlref.Value.End = new ChartPoint(Bars.Time[0],
                                                                 m_uptlref.Value.PriceValue(Bars.Time[0]));
                            m_uptlref.Value.ExtRight = false;
                        }
                        else{
                            if (!m_historytf){
                                m_uptlref.Value.Delete();
                            }
                        }
                    }
                    m_uptlref.Value =
                        DrwTrendLine.Create(
                            new ChartPoint(m_swlotime[0][((m_index.Value))], m_swloval[0][((m_index.Value))]),
                            new ChartPoint(m_swlotime[0][0], m_swloval[0][0]));
                    if (m_uptlbreak.Value){
                        m_uptlbreak.Value = false;
                    }
                    m_uptlendbar.Value = (m_barnum - swlostrength);
                    m_uptlref.Value.ExtLeft = false;
                    m_uptlref.Value.ExtRight = true;
                    m_uptlref.Value.Color = uptlcolor;
                    if ((m_alerttypecaps == "ONCLOSE")){
                        m_uptlref.Value.Alert = EAlertType.BreakoutOnClose;
                    }
                    else{
                        m_uptlref.Value.Alert = (m_alerttypecaps == "INTRABAR") ? EAlertType.BreakoutIntrabar : EAlertType.Disabled;
                    }
                }
            }
            if ((((m_dntlref.Value != null && m_dntlref.Value.Exist)
                  && (m_dntlbreak.Value == false))
                 && m_barnum > (m_dntlendbar.Value + swhistrength)
                 + barspast)
                && PublicFunctions.DoubleGreater(Bars.Close[barspast], m_dntlref.Value.PriceValue(Bars.Time[0]))){
                m_dntlref.Value.End = new ChartPoint(Bars.Time[0], m_dntlref.Value.PriceValue(Bars.Time[0]));
                m_dntlref.Value.ExtRight = false;
                m_dntlbreak.Value = true;
            }
            if ((((m_uptlref.Value != null && m_uptlref.Value.Exist
                   && (m_uptlbreak.Value == false))
                  && m_barnum > (m_uptlendbar.Value + swlostrength)
                  + barspast)
                 && PublicFunctions.DoubleLess(Bars.Close[barspast], m_uptlref.Value.PriceValue(Bars.Time[0])))){
                m_uptlref.Value.End = new ChartPoint(Bars.Time[0], m_uptlref.Value.PriceValue(Bars.Time[0]));
                m_uptlref.Value.ExtRight = false;
                m_uptlbreak.Value = true;
            }
        }
    }
}