using System;
using System.Collections.Generic;
using System.Text;

using SizeDoesNotMatter;

namespace Lab2.Algorithms {
	public class PolardFactorization {
		private OmgNum m_current;

		private int m_iterations;
		public OmgNum FindFactor (OmgNum n, int maxIterations = -1 ) {
			m_iterations = 0;

			m_current = n;

			OmgNum x = OmgOp.Random(1.ToOmgNum(), OmgOp.Subtract(n, 2.ToOmgNum()));
			OmgNum y = 1.ToOmgNum();

			OmgNum i = 0.ToOmgNum();
			OmgNum stage = 2.ToOmgNum();

			OmgNum sub = OmgOp.Subtract(x, y).MakeAbs();
			OmgNum gcd = OmgOp.Gcd(n, sub);
			sub.Release();

			while( gcd.IsOne() ) {
				if( maxIterations >= 0 && m_iterations++ > maxIterations ) {
					return null;
				}

				if( OmgOp.Equal(i, stage) ) {
					y.Release();
					y = new OmgNum(x);
					stage.MultByTwo();
				}

				OmgNum xSquare = OmgOp.Multiply(x, x).Inc();
				x.Release();
				x = OmgOp.Mod(xSquare, n);
				xSquare.Release();

				i.Inc();

				sub = OmgOp.Subtract(x, y).MakeAbs();
				gcd.Release();
				gcd = OmgOp.Gcd(n, sub);
				sub.Release();
			}

			return gcd;
		}


	}
}
