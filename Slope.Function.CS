using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class SlopeMath : FunctionSeries<System.Double>
		{
			public SlopeMath(CStudyControl _master) : base(_master) {  }
			public SlopeMath(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
	        public ISeries<Double> Price { get; set; }
	        public int Length { get; set; }
			
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
				double SumXY = 0.0;
				double SumX = 0.0;
				double SumY = 0.0;
				double SumXSqR = 0.0;
				
				var n = Length;
				
				for (int i = 1; i <= n; i++)
				{
					SumXY += i * Price[n - i];
					SumX += i;
					SumY += Price[n - i];
					SumXSqR += i * i;
				}
				
				double slope = (SumXY -(SumX * SumY) / n) / (SumXSqR - (SumX * SumX) / n);
				
				return slope;
			}
		}
	}
}