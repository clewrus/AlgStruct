using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Subtracter {
		private OmgNum m_subRes;
		private RawNum m_subRaw;

		internal OmgNum Subtract( OmgNum left, OmgNum right ) {
			if( left.IsNegative != right.IsNegative ) {
				throw new OmgFailException("logic error");
			}

			m_subRes = OmgPool.GetZero();
			m_subRaw = m_subRes.Raw;

			_CalcSubtraction(left, right);

			_RemoveLeadingZeros();
			return m_subRes;
		}

		private void _CalcSubtraction( OmgNum left, OmgNum right ) {
			bool flipSign = (left.IsNegative == true);
			bool leftIsLess = _LeftAbsIsLess(left, right);

			if (leftIsLess) {
				m_subRes.IsNegative = !flipSign;
				_SubtractBiggerLess(right.Raw, left.Raw);
			} else {
				m_subRes.IsNegative = flipSign;
				_SubtractBiggerLess(left.Raw, right.Raw);
			}
		}

		private void _SubtractBiggerLess( RawNum left, RawNum right ) {
			const UInt32 leadingBit = (1 << 16);

			UInt32 z = 0;
			for( int i = 0; i < left.Size; i++ ) {
				UInt32 rightDig = (i < right.Size) ? right.Digits[i] : 0;
				UInt32 subVal = (leadingBit | left.Digits[i]) - rightDig - z;
				m_subRaw.Digits.Add(subVal & UInt16.MaxValue);

				z = (~subVal & leadingBit) >> 16;
			}
		}

		private bool _LeftAbsIsLess( OmgNum left, OmgNum right ) {
			var absLeft = new OmgNum(left.Raw);
			var absRight = new OmgNum(right.Raw);

			return OmgOp.Less(absLeft, absRight);
		}

		private void _RemoveLeadingZeros() {
			int zeroSuffix = 0;

			for( int i = m_subRaw.Size - 1; i >= 0 && m_subRaw.Digits[i] == 0; i-- ) {
				zeroSuffix++;
			}
			
			m_subRaw.Digits.RemoveRange(m_subRaw.Size - zeroSuffix, zeroSuffix);
		}
	}
}
