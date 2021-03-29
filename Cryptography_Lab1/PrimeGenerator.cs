using SizeDoesNotMatter;
using SizeDoesNotMatter.PrimeTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cryptography_Lab1 {
	public class PrimeGenerator {
		private MillerRabinTester m_primeTester;

		public PrimeGenerator() {
			m_primeTester = new MillerRabinTester();
		}

		public OmgNum GeneratePrime( int bitLength ) {
			if( bitLength < 0 ) {
				return null;
			}

			if( bitLength == 0 ) {
				return 0.ToOmgNum();
			}

			int numOfTests = Math.Min(4, bitLength);
			OmgNum randNum = OmgOp.Random(bitLength);

			while(_TestFailed(randNum, numOfTests)) {
				randNum.Release();
				randNum = OmgOp.Random(bitLength);
			}

			return randNum;
		}

		private bool _TestFailed(OmgNum num, int numOfTests) {
			m_primeTester.SetTestedNumber(num.Copy());

			for (int i = 0; i < numOfTests; i++) {
				OmgNum testBase = OmgOp.Random(OmgNum.GetConst(2), num);
				bool isPrime = m_primeTester.IsPrimeToBase(testBase);
				testBase.Release();

				if( !isPrime ) {
					return true;
				}
			}

			return false;
		}
	}
}
