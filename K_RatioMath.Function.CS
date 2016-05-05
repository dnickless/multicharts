using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PowerLanguage
{
	namespace Function
	{
		public sealed class K_RatioMath : FunctionSimple<System.Double>
		{
			public K_RatioMath(CStudyControl _master) : base(_master) {  }
			public K_RatioMath(CStudyControl _master, int _ds) : base(_master, _ds) { }
			
			private List<Double> m_OpenEquity;
			public int NumberOfDataBarsInCalendarYear { get; set; }

			protected override void Create()
			{
				// create variable objects and function objects
				m_OpenEquity = new List<Double>();
			}

			protected override void StartCalc()
			{
				// assign inputs
				m_OpenEquity.Clear();
				m_OpenEquity.Add(0);
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
				//if (Bars.Status == EBarState.Close) {
					var portfolio = MyStrategy.Portfolio;
					m_OpenEquity.Add(portfolio.NetProfit + portfolio.OpenPositionProfit);
				//}
				
				if(m_OpenEquity.Count < 4 || !Bars.LastBarOnChart) {
					return 0;
				}
				
				double SumXY = 0.0;
				double SumX = 0.0;
				double SumY = 0.0;
				double SumXSqR = 0.0;
				
				var n = m_OpenEquity.Count - 1;//Bars.CurrentBar;
				
				for (int i = 1; i <= n; i++)
				{
					var openEquity = m_OpenEquity[i];
					SumXY += i * openEquity;
					SumX += i;
					SumY += openEquity;
					SumXSqR += i * i;
				}
				
				double slope = (SumXY -(SumX * SumY) / n) / (SumXSqR - (SumX * SumX) / n);
				return slope;
				
				double Beta0 = (SumY / n) - slope * (SumX / n);
				
				double SumResidSqR = 0.0;
				double StandardErrB1Denom = 0.0;
				
				for (int i = 1; i <= n; i++)
				{
					double YProjected = Beta0 + (slope * i);
					SumResidSqR += Math.Pow(m_OpenEquity[i] - YProjected, 2);
					StandardErrB1Denom += Math.Pow((double)i - (n + 1) / 2.0, 2); 
				}
				
				double SigmaRegress = Math.Sqrt(SumResidSqR / ((double)n - 2));
				double StandardErrB1 = SigmaRegress / Math.Sqrt(StandardErrB1Denom);
				
				double KRatio = 0.0;
				
				if (NumberOfDataBarsInCalendarYear > 1)
				{
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
					if (StandardErrB1 != 0){ 
						KRatio = slope /(StandardErrB1 * n);
					}
				}
				else if (NumberOfDataBarsInCalendarYear == 1)
				{ // Consider only slope / StdErr
					// 1996
					if (StandardErrB1 != 0){
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
			}
		}
	}
}