using SizeDoesNotMatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2.Algorithms {
	public class Factorization {
		private readonly int[] smallPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 1033663387 };
		private OmgNum[] smallPrimesOmg = null;
		private PolardFactorization m_polard = new PolardFactorization();

		public OmgNum Mobius( OmgNum num ) {
			var factorization = Factorize(num);
			if( factorization.Values.Any(x => !x.IsOne()) ) {
				return 0.ToOmgNum();
			}

			return (factorization.Count % 2 == 0) ? 1.ToOmgNum() : (-1).ToOmgNum();
		}

		public OmgNum EulerFunction( OmgNum num ) {
			var factorization = Factorize(num);

			var one = 1.ToOmgNum();

			OmgNum result = 1.ToOmgNum();

			foreach( var factor in factorization ) {
				OmgNum phiP = null;

				if( factor.Value.IsOne() ) {
					phiP = OmgOp.Subtract( factor.Key, one);
				} else {
					var pTonMinusOne = OmgOp.Pow(factor.Key, OmgOp.Subtract(factor.Value, one));
					phiP = OmgOp.Multiply(pTonMinusOne, OmgOp.Subtract(factor.Key, one));
					pTonMinusOne.Release();
				}			
				var nwRes = OmgOp.Multiply(result, phiP);

				result.Release();
				phiP.Release();
				
				result = nwRes;
			}

			return result;
		}

		public Dictionary<OmgNum, OmgNum> Factorize ( OmgNum num ) {
			var initNum = num.Copy();
			var result = new Dictionary<OmgNum, OmgNum>();

			if( _IsSmallPrime( result, num ) ) {
				return result;
			}

			num = _FindFactorsWithPollard( result, num );
			num = _FermaFactorization( result, num );

			if( !num.IsOne()) {
				result[num] = 1.ToOmgNum();
			}

			if (result.Count == 1 && result.TryGetValue(initNum, out var power) && power.IsOne())  {
				return result;
			}

			var nonPrimeFactors = new Stack<OmgNum>(result.Keys);
			while(nonPrimeFactors.Count > 0) {
				var factor = nonPrimeFactors.Pop();
				if( result[factor].IsZero() ) {
					result.Remove(factor);
					continue;
				}

				var primeFactors = Factorize(factor);
				if( primeFactors.Count == 1 && primeFactors.Values.First().IsOne())  { continue; }

				var topPower = result[factor];
				result.Remove(factor);

				foreach( var primeFactor in primeFactors ) {
					var pow = OmgOp.Multiply(topPower, primeFactor.Value);
					if (result.ContainsKey(primeFactor.Key)) {
						result[primeFactor.Key] = OmgOp.Add(result[primeFactor.Key], pow);
					} else {
						result[primeFactor.Key] = pow;
					}
				}
			}

			var keys = new List<OmgNum>(result.Keys);
			foreach( var key in keys ) {
				if( result[key].IsZero() ) {
					result.Remove(key);
				}
			}

			return result;
		}

		private bool _IsSmallPrime(Dictionary<OmgNum, OmgNum> result, OmgNum num) {
			smallPrimesOmg ??= smallPrimes.Select(x => x.ToOmgNum()).ToArray();

			foreach ( var prime in smallPrimesOmg ) {
				if( OmgOp.Equal(prime, num) ) {
					result[prime] = 1.ToOmgNum();
					return true;
				}
			}

			return false;
		}

		private OmgNum _FindFactorsWithPollard( Dictionary<OmgNum, OmgNum> result, OmgNum num ) {
			num = num.Copy();

			while( !num.IsOne() ) {
				OmgNum factor = m_polard.FindFactor(num, maxIterations: 100000);
				if (factor == null || factor.IsOne() || OmgOp.Equal(factor, num)) {
					return num;
				}

				OmgNum pow = _FindPow(ref num, factor);
				result[factor] = pow;
			}

			return num;
		}

		private OmgNum _FermaFactorization(Dictionary<OmgNum, OmgNum> result, OmgNum num) {
			OmgNum factor = null;
			if( OmgOp.DivMod(num, 2.ToOmgNum()).mod.IsZero() ) {
				factor = 2.ToOmgNum();
			}

			if( factor == null ) {
				OmgNum a = OmgOp.Sqrt(num);
				OmgNum aSq = OmgOp.Multiply(a, a);

				bool isSquare = OmgOp.Equal(num, aSq);
				if (isSquare) {
					factor = a;
				}

				a.Inc();
				aSq = OmgOp.Multiply(a, a);
				var s = OmgOp.Subtract(aSq, num);

				while (factor == null) {
					var testSqrt = OmgOp.Sqrt(s);
					if( OmgOp.Equal(s, OmgOp.Multiply(testSqrt, testSqrt))) {
						factor = (OmgOp.Equal(testSqrt, a)) ? OmgOp.Add(a, a) : OmgOp.Subtract(a, testSqrt);
						break;
					}

					a.Inc();
					aSq = OmgOp.Multiply(a, a);
					s = OmgOp.Subtract(aSq, num);
				}
			}

			if( factor == null || factor.IsOne() ) {
				return num;
			}

			OmgNum pow = _FindPow(ref num, factor);
			if( result.ContainsKey(factor) ) {
				result[factor] = OmgOp.Add(result[factor], pow);
			} else {
				result[factor] = pow;
			}

			return num;
		}

		private OmgNum _FindPow( ref OmgNum num, OmgNum factor ) {
			OmgNum pow = 0.ToOmgNum();

			while (true) {
				var divMod = OmgOp.DivMod(num, factor);
				if (!divMod.mod.IsZero()) { break; }
				pow.Inc();

				divMod.mod.Release();
				num.Release();

				num = divMod.div;
			}

			if( pow.IsZero() ) {
				throw new Exception();
			}

			return pow;
		}
	}
}
