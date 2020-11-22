using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Divider {
		private OmgNum m_divRes;
		private OmgNum m_modRes;

		private RawNum m_divRaw;
		private RawNum m_modRaw;

		private RawNum m_buffer;

		private int m_modLeadingZeros;

		public (OmgNum div, OmgNum mod) DivMod( OmgNum left, OmgNum right ) {
			_InitializeDivModResult();
			bool isNegative = left.IsNegative != right.IsNegative;
			m_divRes.IsNegative = isNegative;

			_FindDivModSolution( left.Raw, right.Raw );

			if(isNegative) {
				_Incriment(m_divRaw);
				_Subtract(right.Raw, m_modRaw);
			}

			_RemoveModLeadingZeros();
			return (m_divRes, m_modRes);
		}

		public OmgNum Mod( OmgNum left, OmgNum right ) {
			_InitializeModResult();
			bool isNegative = left.IsNegative != right.IsNegative;

			_FindModSolution(left.Raw, right.Raw);

			if(isNegative) {
				_Subtract(right.Raw, m_modRaw);
			}

			_RemoveModLeadingZeros();
			return m_modRes;
		}

		private void _FindDivModSolution( RawNum left, RawNum right ) {
			if( _IsTrivial( left, right ) ) {
				m_modRaw.CopyFrom(left);
				return;
			}

			m_buffer = OmgPool.GetRawZero();
			_CalcDivMod(left, right);
			OmgPool.ReleaseNumber(m_buffer);

			m_divRaw.Digits.Reverse();
			m_modRaw.Digits.Reverse();
		}

		private void _FindModSolution( RawNum left, RawNum right ) {
			if( _IsTrivial(left, right) ) {
				m_modRaw.CopyFrom(left);
				return;
			}

			m_buffer = OmgPool.GetRawZero();
			_CalcMod(left, right);
			OmgPool.ReleaseNumber(m_buffer);

			m_modRaw.Digits.Reverse();
		}

		private bool _IsTrivial( RawNum left, RawNum right ) {
			OmgNum absLeft = new OmgNum(left);
			OmgNum absRight = new OmgNum(right);

			return OmgOp.Less(absLeft, absRight);
		}

		private void _CalcDivMod( RawNum left, RawNum right ) {
			int offset = left.Size - right.Size + 1;
			_CopyHead(left, right.Size - 1, m_modRaw);

			while( offset > 0 ) {
				m_modRaw.Digits.Add( left.Digits[--offset] );

				if( _ModLessThan( right ) ) {
					_AddZeroToResult();
				} else {
					UInt16 divDigit = _FindDivDigit(right);
					_MultiplyByDigit(right, divDigit, m_buffer);

					_SubtractFromMod(m_buffer);
					m_divRaw.Digits.Add(divDigit);
				}
			}
		}

		private void _CalcMod( RawNum left, RawNum right ) {
			int offset = left.Size - right.Size + 1;
			_CopyHead(left, right.Size - 1, m_modRaw);

			while (offset > 0) {
				m_modRaw.Digits.Add(left.Digits[--offset]);

				if( !_ModLessThan(right) ) {
					UInt16 divDigit = _FindDivDigit(right);
					_MultiplyByDigit(right, divDigit, m_buffer);
					_SubtractFromMod(m_buffer);
				}
			}
		}

		private bool _ModLessThan( RawNum right, bool strictly = true ) {
			if( m_modRaw.Size - m_modLeadingZeros != right.Size ) {
				return m_modRaw.Size - m_modLeadingZeros < right.Size ;
			}

			int commonSize = right.Size;
			for( int i = m_modLeadingZeros; i < m_modRaw.Size; i++ ) {
				int rightIndex = commonSize - (i - m_modLeadingZeros) - 1;
				if (m_modRaw.Digits[i] != right.Digits[rightIndex] ) {
					return m_modRaw.Digits[i] < right.Digits[rightIndex];
				}
			}

			return !strictly;
		}

		private UInt16 _FindDivDigit( RawNum right ) {
			UInt32 down = 0, upper = UInt16.MaxValue;
			UInt32 answer = 0;
			while( upper >= down ) {
				UInt32 tested = (upper + down) >> 1;
				_MultiplyByDigit(right, tested, m_buffer);

				if( _ModLessThan( m_buffer, strictly: false ) ) {
					upper = tested - 1;
				} else {
					answer = tested;
					down = tested + 1;
				}
			}

			return (UInt16)answer;
		}

		private void _MultiplyByDigit( RawNum num, UInt32 digit, RawNum dest ) {
			dest.Clear();
			UInt32 overfit = 0;
			foreach( UInt32 d in num.Digits ) {
				UInt32 mulRes = d * digit + overfit;
				overfit = mulRes >> 16;
				dest.Digits.Add(mulRes & UInt16.MaxValue);
			}

			if( overfit > 0 ) {
				dest.Digits.Add(overfit);
			}
		}

		private void _SubtractFromMod( RawNum subtrahend ) {
			const UInt32 leadingBit = (1 << 16);

			UInt32 z = 0;
			for( int i = 0; i < m_modRaw.Size - m_modLeadingZeros; i++ ) {
				UInt32 modDig = m_modRaw.Digits[m_modRaw.Size - i - 1];
				UInt32 subDig = (i < subtrahend.Size) ? subtrahend.Digits[i] : 0;

				UInt32 sub = (leadingBit | modDig) - subDig - z;
				m_modRaw.Digits[m_modRaw.Size - i - 1] = sub & UInt16.MaxValue;

				z = (~sub & leadingBit) >> 16;
			}
			
			for(int i = m_modLeadingZeros; i < m_modRaw.Size && m_modRaw.Digits[i] == 0; i++) {
				m_modLeadingZeros++;
			}
		}

		private void _AddZeroToResult() {
			if( m_divRaw.Size > 0 ) {
				m_divRaw.Digits.Add(0);
			}
		}

		private void _InitializeModResult() {
			m_modRes = OmgPool.GetZero();
			m_modRaw = m_modRes.Raw;
			m_modLeadingZeros = 0;
		}

		private void _InitializeDivModResult() {
			m_divRes = OmgPool.GetZero();
			m_divRaw = m_divRes.Raw;

			_InitializeModResult();
		}

		private void _CopyHead( RawNum source, int length, RawNum dest ) {
			dest.Clear();
			for( int i = 1; i <= length; i++ ) {
				int sourceIndex = source.Size - i;
				var sourceDigit = source.Digits[sourceIndex];
				dest.Digits.Add(sourceDigit);
			}
		}

		private void _RemoveModLeadingZeros() {
			int zeroSuffix = 0;
			for( int i = m_modRaw.Size - 1; i >= 0 && m_modRaw.Digits[i] == 0; i--) {
				zeroSuffix++;
			}
			m_modRaw.Digits.RemoveRange(m_modRaw.Size - zeroSuffix, zeroSuffix);
		}

		private void _Incriment( RawNum num ) {
			UInt32 overfit = 1;
			for( int i = 0; i < num.Size && overfit > 0; i++ ) {
				UInt32 incRes = num.Digits[i] + overfit;
				num.Digits[i] = incRes & UInt16.MaxValue;
				overfit = incRes >> 16;
			}

			if(overfit > 0) {
				num.Digits.Add(1);
			}
		}

		private void _Subtract( RawNum left, RawNum right ) {
			const UInt32 leadingBit = (1 << 16);
			UInt32 z = 0;

			for(int i = 0; i < left.Size; i++) {
				UInt32 rightDig = (i < right.Size) ? right.Digits[i] : 0;

				UInt32 sub = (leadingBit | left.Digits[i]) - rightDig - z;
				right.Digits[i] = sub & UInt16.MaxValue;

				z = (~sub & leadingBit) >> 16;
			}
		}
	}
}
