using System;
using System.Drawing;
using System.Linq;


namespace PowerLanguage
{
	namespace Function
	{
		public sealed class CRTDRIndicatorMath : FunctionSimple<System.Double>
		{
			public CRTDRIndicatorMath(CStudyControl _master) : base(_master) { }
			public CRTDRIndicatorMath(CStudyControl _master, int _ds) : base(_master, _ds) { }

			protected override void Create()
			{
				// create variable objects and function objects
			}

			protected override void StartCalc()
			{
				// assign inputs
			}

			protected override System.Double CalcBar()
			{
				// function logic
				var high = Bars.High[0];
				var low = Bars.Low[0];
				var close = Bars.Close[0];
				
				return CalcNextValue(high, low, close);
			}
			
			public static double CalcNextValue(double high, double low, double close)
			{
				return high != low ? (close - low) / (high - low) : 1d;
			}
		}
	}
}