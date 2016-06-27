using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using PowerLanguage.Function;
using ATCenterProxy.interop;

namespace PowerLanguage.Strategy
{
    public class Portfolio_KRatioOptimization : PortfolioSignalObject
    {
		private K_RatioMath m_KRatio;
		
        public Portfolio_KRatioOptimization(object _ctx)
            : base(_ctx)
        {
        }

        protected override void Create()
        {
			m_KRatio = new K_RatioMath(this);
        }

        protected override void StartCalc()
        {
            if (Environment.ApplicationCode != EApplicationCode.Portfolio)
			{
                ExecControl.Abort("Signal can be applied in portfolio only.");
			}
			
            m_KRatio.NumberOfDataBarsInCalendarYear = 252;
        }
		
        protected override void CalcBar()
        {
			if(Environment.Optimizing)
			{
				var result = m_KRatio.Value;
				if (Bars.LastBarOnChart)
				{
	            	CustomFitnessValue = result;
				}
			}
        }
    }
}