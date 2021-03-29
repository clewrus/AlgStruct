using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.PrimeTesting {
	public class MillerRabinTester {
		private SmallPrimeTester m_smallPrimeTester;

		private OmgNum m_initialTested;
		private OmgNum m_powerOfTwo;

		private OmgNum m_tested;
		private bool m_shurelyNotAPrime;

		public MillerRabinTester() {
			m_smallPrimeTester = new SmallPrimeTester();
		}

		public void SetTestedNumber( OmgNum num ) {
			m_tested?.Release();
			m_tested = num;

			m_shurelyNotAPrime = OmgOp.Less(num, OmgNum.GetConst(2));
			m_shurelyNotAPrime = m_shurelyNotAPrime || !m_smallPrimeTester.IsPrime(m_tested);

			if (!m_shurelyNotAPrime && OmgOp.Greater(num, OmgNum.GetConst(1))) {
				_FactorPowerOfTwo(num);
			}
		}

		public bool IsPrimeToBase( OmgNum testBase ) {
			if (OmgOp.Less(m_tested, OmgNum.GetConst(1))) {
				return false;
			}

			if (m_shurelyNotAPrime) {
				return false;
			}

			bool result = _MakeTest(testBase);

			return result;
		}

		private void _FactorPowerOfTwo(OmgNum num) {
			m_powerOfTwo?.Release();
			m_initialTested?.Release();

			var n = OmgOp.Subtract(num, OmgNum.GetConst(1));
			m_powerOfTwo = 0.ToOmgNum();

			(OmgNum div, OmgNum mod) divMod = OmgOp.DivMod(n, OmgNum.GetConst(2));
			while( divMod.mod.IsZero() ) {
				n.Release();
				divMod.mod.Release();

				n = divMod.div;
				m_powerOfTwo.Inc();

				divMod = OmgOp.DivMod(n, OmgNum.GetConst(2));
			}

			divMod.div.Release();
			divMod.mod.Release();

			m_initialTested = n;
		}

		private bool _MakeTest(OmgNum testBase ) {
			OmgNum numDec = (new OmgNum(m_tested)).Dec();
			OmgNum tested = OmgOp.Pow(testBase, m_initialTested, m_tested);
			OmgNum iterationsLeft = new OmgNum(m_powerOfTwo);

			try {
				if (OmgOp.Equal(tested, OmgNum.GetConst(1)) || OmgOp.Equal(tested, numDec)) {
					return true;
				}

				while (!iterationsLeft.IsZero()) {
					iterationsLeft.Dec();

					var nwTested = OmgOp.Multiply(tested, tested, m_tested);
					tested.Release();
					tested = nwTested;

					if (OmgOp.Equal(tested, numDec)) {
						return true;
					}
				}

				return false;
			}
			finally {
				numDec.Release();
				tested.Release();
				iterationsLeft.Release();
			}			
		}
	}
}
