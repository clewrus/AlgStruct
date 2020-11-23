using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Adder {
		private const UInt32 c_mask16bit = (UInt32)(UInt16.MaxValue);
		private RawNum m_result;

		public RawNum Add (RawNum left, RawNum right ) {
			m_result = OmgPool.GetRawZero();

			_Add(left, right);
			return m_result;
		}

		private void _Add (RawNum left, RawNum right ) {
			int commonSize = Math.Min(left.Size, right.Size);

			UInt32 overfit = 0;
			for( int i = 0; i < commonSize; i++) {
				UInt32 summ = left.Digits[i] + right.Digits[i] + overfit;
				m_result.Digits.Add(summ & c_mask16bit);
				overfit = summ >> 16;
			}

			RawNum longer = 
				(left.Size > commonSize) ? left : 
				(right.Size > commonSize) ? right : 
				null;

			if( longer != null ) {
				for( int i = commonSize; i < longer.Size; i++ ) {
					UInt32 summ = longer.Digits[i] + overfit;
					m_result.Digits.Add(summ & c_mask16bit);
					overfit = summ >> 16;
				}
			}

			if( overfit > 0 ) {
				m_result.Digits.Add(overfit);
			}
		}
	}
}
