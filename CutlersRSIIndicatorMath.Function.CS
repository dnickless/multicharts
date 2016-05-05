using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class CutlersRSIIndicatorMath : FunctionSimple<System.Double>
		{
			[Input]
			public int Length { get; set; }
			
			double lastClose;
			int c;
			int n;
			double[] lastGains;
			double[] lastLosses;
			
			public CutlersRSIIndicatorMath(CStudyControl _master) : base(_master) { Length = 3; }
			public CutlersRSIIndicatorMath(CStudyControl _master, int _ds) : base(_master, _ds) { Length = 3; }

			protected override void StartCalc()
			{
				lastGains = new double[Length];
				lastLosses = new double[Length];
				n = Length;
				c = 0;
				lastClose = 0;
			}

			public double CalcNextValue(double close)
			{
				double res;
				var lastResult = close - lastClose;
				var i = c % n;
				lastGains[i] = lastResult > 0 ? lastResult : 0;
				lastLosses[i] = lastResult < 0 ? lastResult : 0;
				
				c++;
				if (c > n)
				{
					var avgLoss = lastLosses.Average();
					if (avgLoss == 0)
					{
						 res = 100;
					}
					else
					{
						res = 100.0-100.0/(1+(Math.Abs(lastGains.Average())/Math.Abs(lastLosses.Average())));
					}
					
				}
				else
				{
					 res = 0;
				}
				
				lastClose = close;
				
				return res;
			}
			
			protected override System.Double CalcBar()
			{
				// function logic
				double close = Bars.Close[0];
				return CalcNextValue(close);
			}
		}
	}
}