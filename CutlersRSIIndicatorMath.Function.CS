using System;
using System.Drawing;
using System.Linq;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class CutlersRSIIndicatorMath : FunctionSeries<System.Double>
		{
			[Input]
			public int Length { get; set; }
			
			VariableSeries<double> lastGains;
			VariableSeries<double> lastLosses;
			
			public CutlersRSIIndicatorMath(CStudyControl _master) : base(_master) { Length = 3; }
			public CutlersRSIIndicatorMath(CStudyControl _master, int _ds) : base(_master, _ds) { Length = 3; }

			protected override void Create()
			{
				lastGains = new VariableSeries<double>(this);
				lastLosses = new VariableSeries<double>(this);
			}

			private double GetAverage(VariableSeries<double> series)
			{
				double value = 0;
				
				for(int i = Length - 1; i >= 0; i--)
				{
					value += series[i];
				}
				
				return value / (double)Length;
			}
			
			protected override System.Double CalcBar()
			{
				var lastResult = Bars.Close[0] - Bars.Close[1];
				var i = Bars.CurrentBar % Length;
				lastGains.Value = lastResult > 0 ? lastResult : 0;
				lastLosses.Value = lastResult < 0 ? lastResult : 0;
				
				if (Bars.CurrentBar > Length)
				{
					var avgLoss = GetAverage(lastLosses);
					if (avgLoss == 0)
					{
						return 100;
					}
					else
					{
						return 100.0 - 100.0 / (1 + (Math.Abs(GetAverage(lastGains)) / Math.Abs(avgLoss)));
					}
					
				}
				
				return 0;
			}
		}
	}
}