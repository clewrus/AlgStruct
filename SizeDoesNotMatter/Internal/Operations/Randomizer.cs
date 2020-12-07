using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Randomizer {
		private Random m_rand;
		private RawNum m_result;

		internal Randomizer() {
			m_rand = new Random();
		}

		internal RawNum GetRandom( RawNum min, RawNum max ) {
			m_result = OmgPool.GetRawZero();

			bool maxComplete = false;
			bool minComplete = false;

			for( int i = max.Size - 1; i >= 0; i-- ) {
				int digMin = (minComplete || i >= min.Size) ? 0 : (int)min.Digits[i];
				int digMax = (maxComplete) ? (1 << 16) : ((int)max.Digits[i] + 1);

				UInt16 generated = (UInt16)m_rand.Next(digMin, digMax);
				m_result.Digits.Add(generated);

				maxComplete |= generated < (int)max.Digits[i];
				minComplete |= generated > 0;
			}

			m_result.Digits.Reverse();
			while(m_result.Size > 0 && m_result.Digits[m_result.Size - 1] == 0) {
				m_result.Digits.RemoveAt(m_result.Size - 1);
			}

			return m_result;
		}
	}
}
