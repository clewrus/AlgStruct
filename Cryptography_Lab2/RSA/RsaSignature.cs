using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using SizeDoesNotMatter;

using System;
using System.Linq;

namespace Cryptography_Lab2.RSA {
	public static class RsaSignature {
		public static IEnumerable<OmgNum> MakeSignature (string text, MessageConverter converter, SignSecret secret) {
			string hash = _GetHashString(text);
			var nums = converter.ToNumbers(hash);
			return Encode(nums, secret);
		}

		public static bool SignatureIsValid (string decodedText, IEnumerable<OmgNum> signature, MessageConverter converter, SignCertificate knownKey) {
			string hash = _GetHashString(decodedText);
			var nums = converter.ToNumbers(hash);

			var decodedSignature = Decode(signature, knownKey);
			return Enumerable.SequenceEqual(nums, decodedSignature);
		}

		private static IEnumerable<OmgNum> Encode (IEnumerable<OmgNum> message, SignSecret secret) {
			foreach (var num in message) {
				yield return OmgOp.Pow(num, secret.E, secret.N);
			}
		}

		private static IEnumerable<OmgNum> Decode (IEnumerable<OmgNum> message, SignCertificate cert) {
			foreach (var num in message) {
				yield return OmgOp.Pow(num, cert.D, cert.N);
			}
		}

		private static string _GetHashString (string inputString) {
			var sb = new StringBuilder();

			foreach (byte b in _GetHash(inputString)) {
				sb.Append(b.ToString("X2"));
			}

			return sb.ToString();
		}

		private static byte[] _GetHash (string inputString) {
			using (HashAlgorithm algorithm = SHA512.Create()) {
				return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
			}
		}
	}
}
