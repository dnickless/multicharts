using System;
using System.Drawing;
using System.Linq;

namespace PowerLanguage.Function
{
    public sealed class XAverageThatWorks : FunctionSeries<System.Double>
    {
        public XAverageThatWorks(CStudyControl _master) : base(_master) { }
        public XAverageThatWorks(CStudyControl _master, int _ds) : base(_master, _ds) { }

        public ISeries<double> Price { private get; set; }
        public int Length { private get; set; }

        protected override double CalcBar()
        {
            if (Bars.CurrentBar == 1)
            {
                // return Simple Moving Average for the past <Length> days upon first bar
                var value = 0.0;
                for (var i = 0; i < Length; i++)
                    value += Price[i];
                return value / Length;
            }
            else
            {
                var prev = this[1];
                return prev + 2.0 / (Length + 1) * (Price[0] - prev);
            }
        }
    }
}