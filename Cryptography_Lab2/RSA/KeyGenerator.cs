using Cryptography_Lab1;
using SizeDoesNotMatter;

namespace Cryptography_Lab2.RSA {
	public class KeyGenerator {
		private PrimeGenerator m_primeSource;

		public struct Key {
			public PubKey pub;
			public PrivKey priv;
		}

		public struct SignaureKey {
			public SignSecret secret;
			public SignCertificate cert;
		}

		public KeyGenerator () {
			m_primeSource = new PrimeGenerator();
		}

		public SignaureKey GenerateSignature (int keyByteLength) {
			int componentLength = keyByteLength * 8 / 2;

			OmgNum p = m_primeSource.GeneratePrime(componentLength);
			OmgNum q = m_primeSource.GeneratePrime(componentLength);

			OmgNum n = OmgOp.Multiply(p, q);
			OmgNum carmN = _Carmichael(p.Dec(), q.Dec());
			OmgNum e = _SelectRandomExponent(carmN, out OmgNum d);

			p.Release();
			p.Release();

			return new SignaureKey {
				cert = new SignCertificate { N = n.Copy(), D = d },
				secret = new SignSecret() { N = n, E = e }
			};
		}

		public Key GenerateKey (int keyByteLength) {
			int componentLength = keyByteLength * 8 / 2;

			OmgNum p = m_primeSource.GeneratePrime(componentLength);
			OmgNum q = m_primeSource.GeneratePrime(componentLength);

			OmgNum n = OmgOp.Multiply(p, q);

			OmgNum phiP = OmgOp.Subtract(p, OmgNum.GetConst(1));
			OmgNum phiQ = OmgOp.Subtract(q, OmgNum.GetConst(1));
			OmgNum carmN = _Carmichael(phiP, phiQ);

			OmgNum e = _SelectExponent(carmN);

			bool dpExists = OmgOp.TryInverseByMod(e, phiP, out OmgNum dP);
			bool dQExists = OmgOp.TryInverseByMod(e, phiQ, out OmgNum dQ);
			bool qInvExists = OmgOp.TryInverseByMod(q, p, out OmgNum qInv);

			if (!dpExists || !dQExists || !qInvExists) {
				throw new OmgFailException("Inverse is impossible");
			}

			return new Key {
				pub = new PubKey { N = n, E = e },
				priv = new PrivKey { P = p, Q = q, dP = dP, dQ = dQ, qInv = qInv }
			};
		}

		private OmgNum _Carmichael (OmgNum phiP, OmgNum phiQ) {
			OmgNum pq = OmgOp.Multiply(phiP, phiQ);
			OmgNum pqGCD = OmgOp.Gcd(phiP, phiQ);

			OmgNum lcm = OmgOp.Div(pq, pqGCD);

			pq.Release();
			pqGCD.Release();

			return lcm;
		}

		private OmgNum _SelectExponent (OmgNum carmN) {
			OmgNum candidate = ((1 << 16) + 1).ToOmgNum();

			OmgNum gcd = OmgOp.Gcd(candidate, carmN);
			while (!gcd.IsOne()) {
				gcd = OmgOp.Gcd(candidate.Inc(), carmN);
			}

			gcd.Release();
			return candidate;
		}

		private OmgNum _SelectRandomExponent (OmgNum carmN, out OmgNum D) {
			OmgNum e = OmgOp.Random(OmgNum.GetConst(0), carmN);

			while (!OmgOp.TryInverseByMod(e, carmN, out D)) {
				e = OmgOp.Random(OmgNum.GetConst(0), carmN);
			}

			return e;
		}
	}
}
