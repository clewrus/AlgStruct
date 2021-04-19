using System.Collections.Generic;
using SizeDoesNotMatter;

namespace Cryptography_Lab2.RSA {
	public static class RsaUtility {

		public static IEnumerable<OmgNum> Encode (IEnumerable<OmgNum> message, PubKey key) {
			foreach (var num in message) {
				yield return OmgOp.Pow(num, key.E, key.N);
			}
		}

		public static IEnumerable<OmgNum> Decode (IEnumerable<OmgNum> code, PrivKey key) {
			foreach (var num in code) {
				OmgNum m1 = OmgOp.Pow(num, key.dP, key.P);
				OmgNum m2 = OmgOp.Pow(num, key.dQ, key.Q);

				OmgNum mdif = OmgOp.Subtract(m1, m2);
				OmgNum h = OmgOp.Multiply(key.qInv, mdif, key.P);

				OmgNum hq = OmgOp.Multiply(h, key.Q);
				OmgNum m = OmgOp.Add(m2, hq);

				m1.Release();
				m2.Release();
				mdif.Release();
				h.Release();
				hq.Release();

				yield return m;
			}
		}
	}
}
