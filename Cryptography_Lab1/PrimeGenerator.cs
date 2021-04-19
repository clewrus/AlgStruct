using SizeDoesNotMatter;
using SizeDoesNotMatter.PrimeTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography_Lab1 {
	public class PrimeGenerator {
		private int m_testsPerNumber;
		private MillerRabinTester m_primeTester;

		public PrimeGenerator (int testsPerNumber = 64) {
			m_testsPerNumber = testsPerNumber;
			m_primeTester = new MillerRabinTester();
		}

		public OmgNum GeneratePrime (int bitLength) {
			if (bitLength < 0) {
				return null;
			}

			if (bitLength == 0) {
				return 0.ToOmgNum();
			}

			OmgNum randNum = OmgOp.Random(bitLength);

			while (_TestFailed(randNum)) {
				randNum.Release();
				randNum = OmgOp.Random(bitLength);
			}

			return randNum;
		}

		private bool _TestFailed (OmgNum num) {
			m_primeTester.SetTestedNumber(num.Copy());

			for (int i = 0; i < m_testsPerNumber; i++) {
				OmgNum testBase = OmgOp.Random(OmgNum.GetConst(2), num);
				bool isPrime = m_primeTester.IsPrimeToBase(testBase);
				testBase.Release();

				if (!isPrime) {
					return true;
				}
			}

			return false;
		}
	}
}
