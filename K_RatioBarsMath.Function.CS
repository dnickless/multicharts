using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class K_RatioBarsMath : FunctionSeries<System.Double>
		{
			public K_RatioBarsMath(CStudyControl _master) : base(_master) {  }
			public K_RatioBarsMath(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
	        public ISeries<Double> Price { get; set; }
	        public int Length { get; set; }
			
			public int NumberOfDataBarsInCalendarYear { get; set; }

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
				/*
				Function:  K-Ratio 
				Design & Modification Dates: 1996, 2003, 2013
				Designer: Lars Kestner
				Date of Revision: 05/12/2013
				Revised by: Jim "JDog" Helms

				The K-ratio was introduced by Lars Kestner in 1996 as a reward to risk measurement to compliment the popular
				Sharpe Ratio. The K-ratio is calculated by fitting a linear trend series to cumulative returns and
				estimating the slope and variability of slope. Recently, Mr Kestner published an article titled 
				"(Re)Introducing the K-Ratio" where he modified the K-Ratio to include a scaling factor so the ratio 
				would be consistent when comparing equity curves of different time frames: daily versus monthly (i.e., 12 bars 
				for monthly data, 52 bars for weekly data, and circa 252 for daily data).

				K-Ratio = (slope of least-squares regression line / standard error of slope) x 
					(square root of number of observations per calendar year / number of observations) 
					
				The K-Ratio value, in and by itself, is of little concern when optimizing (or comparing)different strategies 
				over the same dates and bar frequency.  The greater the K-Ratio, the better. 

				****}

				//Input of 0 returns the 2003 version of the K-Ratio which excludes the calendar year scaling factor
				//Input of 1 returns the raw Slope / Standard error of slope 
				 
				*/
					double SumXY = 0.0;
					double SumX = 0.0;
					double SumY = 0.0;
					double SumXSqR = 0.0;
					
					var n = Length;
					
					for (int i = 1; i <= n; i++)
					{
						var price = Price[n - i];
						SumXY += i * price;
						SumX += i;
						SumY += price;
						SumXSqR += i * i;
					}
					
					double slope = (SumXY -(SumX * SumY) / n) / (SumXSqR - (SumX * SumX) / n);
					double Beta0 = (SumY / n) - slope * (SumX / n);
					
					double SumResidSqR = 0.0;
					double StandardErrB1Denom = 0.0;
					
					for (int i = 1; i <= n; i++)
					{
						double YProjected = Beta0 + (slope * i);
						SumResidSqR += Math.Pow(Price[n - i] - YProjected, 2);
						StandardErrB1Denom += Math.Pow((double)i - (n + 1) / 2.0, 2); 
					}
					
					double SigmaRegress = Math.Sqrt(SumResidSqR / ((double)n - 2));
					double StandardErrB1 = SigmaRegress / Math.Sqrt(StandardErrB1Denom);
					
					double KRatio = 0.0;
					
					if (NumberOfDataBarsInCalendarYear > 1
						){
						// 2013
						double SqRExpectedObservation = Math.Sqrt(NumberOfDataBarsInCalendarYear);
						if (StandardErrB1 != 0)
						{ 
							KRatio = (slope /StandardErrB1) * (SqRExpectedObservation / n);
						}
					}
					else if (NumberOfDataBarsInCalendarYear == 0)
					{ // Ignore the Calendar Year Calculation
						// 2003
						if (StandardErrB1 != 0)
						{ 
							KRatio = slope /(StandardErrB1 * n);
						}
					}
					else if (NumberOfDataBarsInCalendarYear == 1)
					{ // Consider only slope / StdErr
						// 1996
						if (StandardErrB1 != 0)
						{
							KRatio = slope / (StandardErrB1 * Math.Sqrt(n)); // DH: added  * Math.Sqrt(n) to original code
						}	
					}
					
					/*
					Output.WriteLine("K-Ratio = " + KRatio.ToString());
					Output.WriteLine("Equity  = " + m_OpenEquity[n].ToString());
					Output.WriteLine("Slope   = " + slope.ToString());
					Output.WriteLine("StdErr  = " + StandardErrB1.ToString());
					*/
					
					return(KRatio);
				//}
				
				// function logic
				//return default(System.Double);
			}
		}
	}
}