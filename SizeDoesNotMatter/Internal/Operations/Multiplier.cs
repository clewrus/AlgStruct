using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Multiplier {
		private RawNum m_rightCopy;
		private OmgNum m_powRes;


		private RawNum m_multRaw;

		internal RawNum MultiplyKaratsuba( RawNum left, RawNum right ) {
			if (left.IsZero() || right.IsZero()) {
				return OmgPool.GetRawZero();
			}

			int n = Math.Min(left.Size, right.Size);

			(RawNum head, RawNum tail) leftSep = _SeparateHead(left, n);
			(RawNum head, RawNum tail) rightSep = _SeparateHead(right, n);

			RawNum headsProd = Multiply(leftSep.head, rightSep.head);
			RawNum tailsProd = Multiply(leftSep.tail, rightSep.tail);

			RawNum headTail = Multiply(leftSep.head, rightSep.tail);
			RawNum tailHead = Multiply(leftSep.tail, rightSep.head);

			OmgNum midSum = OmgOp.Add(new OmgNum(headTail), new OmgNum(tailHead));

			m_multRaw = OmgPool.GetRawZero();

			m_multRaw.CopyFrom(tailsProd);
			midSum.Raw.Digits.InsertRange(0, Enumerable.Repeat<uint>(0, n));
			headsProd.Digits.InsertRange(0, Enumerable.Repeat<uint>(0, 2*n));

			OmgNum res = OmgOp.Add(new OmgNum(tailsProd), midSum);
			OmgNum finalRes = OmgOp.Add(new OmgNum(headsProd), res);

			midSum.Release();
			res.Release();
			OmgPool.ReleaseNumber(headTail);
			OmgPool.ReleaseNumber(tailHead);

			return finalRes.Raw;
		}

		internal RawNum Multiply( RawNum left, RawNum right ) {
			m_multRaw = OmgPool.GetRawZero();

			if(left.IsZero() || right.IsZero()) {
				return m_multRaw;
			}

			if( left.Size < right.Size ) {
				_Swap(ref left, ref right);
			}

			_MakeMult(left, right);
			while( m_multRaw.Size > 0 && m_multRaw.Digits[m_multRaw.Size - 1] == 0 ) {
				m_multRaw.Digits.RemoveAt(m_multRaw.Size - 1);
			}

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

			if( mod == null ) {
				return _MakeFastPow(new OmgNum(left), right).Raw;
			} else {
				return _MakeFastPow(new OmgNum(left), right, new OmgNum(mod)).Raw;
			}
		}

		private OmgNum _MakeFastPow( OmgNum left, RawNum right ) {
			if (right.IsZero()) {
				var res = OmgPool.GetRawZero();
				res.Digits.Add(1);
				return new OmgNum(res);
			}

			var rightCopy = OmgPool.GetRawZero();
			rightCopy.CopyFrom(right);

			bool even = (rightCopy.Digits[0] & 1) == 0;
			if (even) {
				rightCopy.DivByTwo();
				var p = _MakeFastPow(left, rightCopy);

				OmgNum powSquare = OmgOp.Multiply(p, p);
				p.Release();

				OmgPool.ReleaseNumber(rightCopy);
				return powSquare;
			} else {
				_SubOne(rightCopy);
				var p = _MakeFastPow(left, rightCopy);

				OmgNum powPlusOne = OmgOp.Multiply(p, left);
				p.Release();

				OmgPool.ReleaseNumber(rightCopy);
				return powPlusOne;
			}
		}

		private OmgNum _MakeFastPow (OmgNum left, RawNum right, OmgNum mod) {

			if(right.IsZero() ) {
				var res = OmgPool.GetRawZero();
				res.Digits.Add(1);
				return new OmgNum(res);
			}

			var rightCopy = OmgPool.GetRawZero();
			rightCopy.CopyFrom(right);

			bool even = (rightCopy.Digits[0] & 1) == 0;
			if (even) {
				rightCopy.DivByTwo();
				var p = _MakeFastPow(left, rightCopy, mod);

				OmgNum powSquare = OmgOp.Multiply(p, p);
				OmgNum powSquareModded = OmgOp.Mod(powSquare, mod);

				powSquare.Release();
				p.Release();

				OmgPool.ReleaseNumber(rightCopy);
				return powSquareModded;
			} else {
				_SubOne(rightCopy);
				var p = _MakeFastPow(left, rightCopy, mod);

				OmgNum powPlusOne = OmgOp.Multiply(p, left);
				OmgNum powPlusOneModded = OmgOp.Mod(powPlusOne, mod);

				powPlusOne.Release();
				p.Release();

				OmgPool.ReleaseNumber(rightCopy);
				return powPlusOneModded;
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
				num.Digits[i] = bitVal & UInt16.MaxValue;
			}

			if (num.Digits[num.Size - 1] == 0) {
				num.Digits.RemoveAt(num.Size - 1);
			}
		}

		private (RawNum head, RawNum tail) _SeparateHead(RawNum num, int n) {
			(RawNum head, RawNum tail) res = (OmgPool.GetRawZero(), OmgPool.GetRawZero());
			for (int i = 0; i < num.Size; i++) {
				if( i < n / 2 ) {
					res.tail.Digits.Add(num.Digits[i]);
				}
				else {
					res.head.Digits.Add(num.Digits[i]);
				}
			}

			return res;
		}
	}
}
