using NUnit.Framework;
using SizeDoesNotMatter;
using SizeDoesNotMatter.PrimeTesting;

namespace Cryptography_Lab1_tests {
	public class Tests {
		private MillerRabinTester m_millerRabin;

		[SetUp]
		public void Setup () {
			m_millerRabin = new MillerRabinTester();
		}

		[Test]
		public void NumberOfPrimesLessThen100base2 () {
			int count = 0;
			for( int i = 1; i < 100; i++ ) {
				m_millerRabin.SetTestedNumber(i.ToOmgNum());
				if (m_millerRabin.IsPrimeToBase(OmgNum.GetConst(2))) {
					count++;
				}
			}
			Assert.AreEqual(count, 24);
		}

		[Test]
		public void NumberOfPrimesLessThen10000base2and3 () {
			int count = 0;
			for (int i = 1; i < 10000; i++) {
				m_millerRabin.SetTestedNumber(i.ToOmgNum());
				if (m_millerRabin.IsPrimeToBase(OmgNum.GetConst(2)) && m_millerRabin.IsPrimeToBase(OmgNum.GetConst(3))) {
					count++;
				}
			}
			Assert.AreEqual(count, 1227);
		}
	}
}