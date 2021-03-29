using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SizeDoesNotMatter.PrimeTesting {
	internal class SmallPrimeTester {
		private static readonly OmgNum[] s_smallPrimes = (new int[] {
			2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47
		}).Select (p => p.ToOmgNum()).ToArray();

		public bool IsPrime( OmgNum tested ) {

			foreach( var prime in s_smallPrimes ) {
				if (OmgOp.Less(tested, prime) || OmgOp.Equal(tested, prime)) {
					return true;
				}

				var mod = OmgOp.Mod(tested, prime);
				bool isComposite = mod.IsZero();

				mod.Release();

				if( isComposite ) {
					return false;
				}
			}

			return true;
		}

	}
}
