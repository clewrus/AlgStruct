using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Gcder {


		internal RawNum FindGcd( RawNum left, RawNum right ) {
			if( left.IsZero() ) {
				RawNum gcd = OmgPool.GetRawZero();
				gcd.CopyFrom(right);
				return gcd;
			}

			var divModRes = OmgOp.DivMod(right.OmgWrapper(), left.OmgWrapper());
			divModRes.div.Release();

			RawNum gcdRes = FindGcd(divModRes.mod.Raw, left);
			divModRes.mod.Release();

			return gcdRes;
		}

		internal RawNum ExtendedGcd( RawNum left, RawNum right, out OmgNum x, out OmgNum y) {
			if (left.IsZero()) {
				x = 0.ToOmgNum();
				y = 1.ToOmgNum();

				RawNum gcd = OmgPool.GetRawZero();
				gcd.CopyFrom(right);
				return gcd;
			}

			var divModRes = OmgOp.DivMod(right.OmgWrapper(), left.OmgWrapper());

			RawNum gcdRes = ExtendedGcd(divModRes.mod.Raw, left, out OmgNum x2, out OmgNum y2);
			divModRes.mod.Release();

			OmgNum multRes = OmgOp.Multiply(divModRes.div, x2);
			divModRes.div.Release();

			x = OmgOp.Subtract(y2, multRes);
			y2.Release();
			multRes.Release();

			y = x2;
			return gcdRes;
		}
	}
}
