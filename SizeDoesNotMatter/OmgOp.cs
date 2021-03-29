using SizeDoesNotMatter.Internal;
using SizeDoesNotMatter.Internal.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter {
	public static class OmgOp {
		private static Adder m_adder = new Adder();
		private static Comparer m_comparer = new Comparer();
		private static Subtracter m_subtracter = new Subtracter();
		private static Divider m_divider = new Divider();
		private static Multiplier m_multiplier = new Multiplier();
		private static Rooter m_rooter = new Rooter();
		private static Gcder m_gcder = new Gcder();
		private static Randomizer m_randomizer = new Randomizer();


		public static OmgNum Add( OmgNum left, OmgNum right ) {
			if( left.IsNegative && right.IsNegative ) {
				return _Negative(m_adder.Add(left.Raw, right.Raw));
			} 
			if( !left.IsNegative && right.IsNegative ) {
				return m_subtracter.Subtract(left, _AbsShell(right));
			}
			if( left.IsNegative && !right.IsNegative ) {
				m_subtracter.Subtract(right, _AbsShell(left));
			}
			return _Positive(m_adder.Add(left.Raw, right.Raw));
		}

		public static OmgNum Add( OmgNum left, OmgNum right, OmgNum mod ) {
			return _BinaryModded(left, right, mod, Add);
		}

		public static OmgNum Subtract( OmgNum left, OmgNum right ) {
			if(!left.IsNegative && !right.IsNegative ||
				left.IsNegative && right.IsNegative
			) {
				return m_subtracter.Subtract(left, right);
			}
			if(!left.IsNegative && right.IsNegative) {
				return _Positive(m_adder.Add(left.Raw, right.Raw));
			}
			return _Negative(m_adder.Add(left.Raw, right.Raw));
		}

		public static OmgNum Subtract (OmgNum left, OmgNum right, OmgNum mod) {
			return _BinaryModded(left, right, mod, Subtract);
		}

		public static OmgNum Multiply( OmgNum left, OmgNum right ) {
			if( left.IsNegative == right.IsNegative ) {
				return _Positive(m_multiplier.Multiply(left.Raw, right.Raw));
			}

			return _Negative(m_multiplier.Multiply(left.Raw, right.Raw));
		}

		public static OmgNum Multiply (OmgNum left, OmgNum right, OmgNum mod) {
			return _BinaryModded(left, right, mod, Multiply);
		}

		public static OmgNum Mod( OmgNum left, OmgNum right ) {
			return m_divider.Mod(left, right);
		}

		public static (OmgNum div, OmgNum mod) DivMod( OmgNum left, OmgNum right ) {
			if( right.IsZero() ) throw new OmgFailException("division by zero");
			return m_divider.DivMod(left, right);
		}

		public static (OmgNum div, OmgNum mod) DivMod( OmgNum left, OmgNum right, OmgNum mod ) {
			var res = _WithModded(left, right, mod, (l, r) => DivMod(l, r));

			var divModded = Mod(res.div, mod);
			var modModded = Mod(res.mod, mod);

			res.div.Release();
			res.mod.Release();

			return (divModded, modModded);
		}

		public static OmgNum Pow (OmgNum left, OmgNum right) {
			bool evenPower = (right.Raw.Digits[0] & 1) == 0;
			if( evenPower || !left.IsNegative) {
				return _Positive(m_multiplier.Pow(left.Raw, right.Raw));
			}

			return _Negative(m_multiplier.Pow(left.Raw, right.Raw));
		}

		public static OmgNum Pow( OmgNum left, OmgNum right, OmgNum mod ) {
			OmgNum moddedLeft = Mod(left, mod);
			OmgNum moddedPow = _Positive(m_multiplier.Pow(left.Raw, right.Raw, mod.Raw));
			moddedLeft.Release();
			return moddedPow;
		}

		public static OmgNum Sqrt( OmgNum left ) {
			if( left.IsZero() ) {
				return OmgPool.GetZero();
			}

			if( left.IsNegative ) {
				throw new OmgFailException("can't square root negative number");
			}

			return _Positive(m_rooter.Sqrt(left.Raw));
		}

		public static OmgNum Gcd( OmgNum left, OmgNum right ) {
			return _Positive(m_gcder.FindGcd(left.Raw, right.Raw));
		}

		public static bool TryInverseByMod( OmgNum left, OmgNum mod, out OmgNum inverce ) {
			RawNum gcd = m_gcder.ExtendedGcd(left.Raw, mod.Raw, out OmgNum x, out OmgNum y);
			y.Release();

			bool result = (gcd.Size == 1) && (gcd.Digits[0] == 1);
			OmgPool.ReleaseNumber(gcd);

			inverce = (result) ? Mod(x, mod) : null;
			x.Release();

			return result;
		}

		public static OmgNum Random( OmgNum min, OmgNum max ) {
			if (Less(max, min)) { return min.Copy();  }
			if( !min.IsNegative && !max.IsNegative ) {
				return _Positive(m_randomizer.GetRandom(min.Raw, max.Raw));
			}

			throw new NotImplementedException();
		}

		public static OmgNum Random( int bitLength ) {
			if( bitLength < 0 ) { throw new OmgFailException("BitLength can't be less than zero."); }
			return _Positive(m_randomizer.GenerateWithBitlength(bitLength));
		}

		public static bool Equal (OmgNum left, OmgNum right) {
			if( left.IsNegative != right.IsNegative ) {
				return left.IsZero() && right.IsZero();
			}

			return m_comparer.Equal(left.Raw, right.Raw);
		}

		public static bool Equal( OmgNum left, OmgNum right, OmgNum mod ) {
			return _WithModded(left, right, mod, (l, r) => Equal(l, r));
		}

		public static bool Greater( OmgNum left, OmgNum right ) {
			return Less(right, left);
		}

		public static bool Greater( OmgNum left, OmgNum right, OmgNum mod ) {
			return Less(right, left, mod);
		}

		public static bool Less( OmgNum left, OmgNum right ) {
			if( left.IsNegative != right.IsNegative ) {
				return left.IsNegative;
			}
			if( !left.IsNegative && !right.IsNegative ) {
				return m_comparer.Less(left.Raw, right.Raw);
			}
			return m_comparer.Less(right.Raw, left.Raw);
		}

		public static bool Less( OmgNum left, OmgNum right, OmgNum mod ) {
			return _WithModded(left, right, mod, (l, r) => Less(l, r)); ;
		}


		private static OmgNum _Positive( RawNum raw ) {
			var result = new OmgNum(raw);
			result.IsNegative = false;

			return result;
		}

		private static OmgNum _Negative( RawNum raw ) {
			var result = new OmgNum(raw);
			result.IsNegative = true;

			return result;
		}

		private static OmgNum _AbsShell (OmgNum number) {
			return new OmgNum(number.Raw);
		}

		private static OmgNum _BinaryModded( OmgNum left, OmgNum right, OmgNum mod, Func<OmgNum, OmgNum, OmgNum> binFunc ) {
			OmgNum res = _WithModded(left, right, mod, binFunc);

			OmgNum resModded = Mod(res, mod);
			res.Release();

			return resModded;
		}

		private static T _WithModded<T>( OmgNum left, OmgNum right, OmgNum mod, Func<OmgNum, OmgNum, T> func ) {
			if( mod == null ) {
				throw new OmgFailException("mod is null");
			}
			if(mod.IsZero()) {
				throw new OmgFailException("mod is zero");
			}

			var leftModded = Mod(left, mod);
			var rightModded = Mod(right, mod);

			T result = func(leftModded, rightModded);

			leftModded.Release();
			rightModded.Release();

			return result;
		}

		
	}
}
