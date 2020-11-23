using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Multiplier {
		private RawNum m_rightCopy;
		private OmgNum m_powRes;


		private RawNum m_multRaw;

		internal RawNum Multiply( RawNum left, RawNum right ) {
			m_multRaw = OmgPool.GetRawZero();

			if(left.IsZero() || right.IsZero()) {
				return m_multRaw;
			}

			if( left.Size < right.Size ) {
				_Swap(ref left, ref right);
			}

			_MakeMult(left, right);

			return m_multRaw;
		}

		internal RawNum Pow( RawNum left, RawNum right, RawNum mod = null ) {
			m_powRes = OmgPool.GetZero();

			if( left.IsZero() ) {
				return m_powRes.Raw;
			}

			if( right.IsZero() ) {
				m_powRes.Raw.Digits.Add(1);
				return m_powRes.Raw;
			}

			m_powRes.Raw.CopyFrom(left);

			m_rightCopy = OmgPool.GetRawZero();
			m_rightCopy.CopyFrom(right);
			_SubOne(m_rightCopy);

			if( mod == null ) {
				_MakeFastPow(new OmgNum(left));
			} else {
				_MakeFastPow(new OmgNum(left), new OmgNum(mod));
			}

			OmgPool.ReleaseNumber(m_rightCopy);
			return m_powRes.Raw;
		}

		private void _MakeFastPow( OmgNum left ) {
			while (m_rightCopy.Size > 0) {
				bool even = (m_rightCopy.Digits[0] & 1) == 0;
				if( even ) {
					OmgNum powSquare = OmgOp.Multiply(m_powRes, m_powRes);
					m_powRes.Release();
					m_powRes = powSquare;
					_DivByTwo(m_rightCopy);
				} else {
					OmgNum powPlusOne = OmgOp.Multiply(m_powRes, left);
					m_powRes.Release();
					m_powRes = powPlusOne;
					_SubOne(m_rightCopy);
				}
			}
		}

		private void _MakeFastPow (OmgNum left, OmgNum mod) {
			while (m_rightCopy.Size > 0) {
				bool even = (m_rightCopy.Digits[0] & 1) == 0;
				if (even) {
					OmgNum powSquare = OmgOp.Multiply(m_powRes, m_powRes);
					OmgNum powSquareModded = OmgOp.Mod(powSquare, mod);

					powSquare.Release();
					m_powRes.Release();

					m_powRes = powSquareModded;

					_DivByTwo(m_rightCopy);
				} else {
					OmgNum powPlusOne = OmgOp.Multiply(m_powRes, left);
					OmgNum powPlusOneModded = OmgOp.Mod(powPlusOne, mod);

					powPlusOne.Release();
					m_powRes.Release();

					m_powRes = powPlusOneModded;

					_SubOne(m_rightCopy);
				}
			}
		}

		private void _MakeMult( RawNum left, RawNum right ) {
			_PumpResWithZeros(left.Size + right.Size - 1);

			UInt32 overflow = 0;
			for (int i = 0; i < right.Size; i++) {
				m_multRaw.Digits[i + left.Size - 1] = overflow;
				overflow = _MakeMultStep(i, right.Digits[i], left);
			}

			if( overflow > 0 ) {
				m_multRaw.Digits.Add(overflow);
			}
		}

		private UInt32 _MakeMultStep( int offset, UInt32 digit, RawNum left) {
			UInt32 overflow = 0;

			for (int j = 0; j < left.Size; j++) {
				UInt32 digRes = m_multRaw.Digits[offset + j] + digit * left.Digits[j] + overflow;
				m_multRaw.Digits[offset + j] = digRes & UInt16.MaxValue;
				overflow = digRes >> 16;
			}

			return overflow;
		}

		private void _PumpResWithZeros( int zeroCount ) {
			for( int i = 0; i < zeroCount; i++ ) {
				m_multRaw.Digits.Add(0);
			}
		}

		private void _Swap<T>( ref T left, ref T right ) {
			T temp = left;
			left = right;
			right = temp;
		}

		private void _SubOne( RawNum num ) {
			UInt32 z = 1;
			for (int i = 0; i < num.Size && z > 0; i++) {
				UInt32 bitVal = ((1 << 16) | num.Digits[i]) - z;
				z = (~bitVal & (1 << 16)) >> 16;
			}

			if (num.Digits[num.Size - 1] == 0) {
				num.Digits.RemoveAt(num.Size - 1);
			}
		}

		private void _DivByTwo( RawNum num ) {
			UInt32 leadingBit = 0;
			for(int i = num.Size - 1; i >= 0; i--) {
				UInt32 bitVal = num.Digits[i];
				num.Digits[i] = (leadingBit << 15) | (bitVal >> 1);
				leadingBit = bitVal & 1;
			}
			if(num.Digits[num.Size - 1] == 0) {
				num.Digits.RemoveAt(num.Size - 1);
			}
		}
	}
}
