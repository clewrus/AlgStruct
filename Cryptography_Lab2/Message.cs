using System;
using System.Collections.Generic;
using System.Text;
using SizeDoesNotMatter;
using Cryptography_Lab2.RSA;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Cryptography_Lab2 {
	public class Message {
		public byte[] message;
		public byte[] signature;

		public static Message FromNumbers (IEnumerable<OmgNum> message, IEnumerable<OmgNum> signature) {
			var messageBytes = message.Select(num => num.ToByteArray()).ToList();
			var signatureBytes = signature.Select(num => num.ToByteArray()).ToList();

			var m = new Message();

			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream()) {
				bf.Serialize(ms, messageBytes);
				m.message = ms.ToArray();
			}

			using (var ms = new MemoryStream()) {
				bf.Serialize(ms, signatureBytes);
				m.signature = ms.ToArray();
			}

			return m;
		}

		public IEnumerable<OmgNum> GetMessage () {
			return _BytesToNums(message);
		}

		public IEnumerable<OmgNum> GetSignature () {
			return _BytesToNums(signature);
		}

		private IEnumerable<OmgNum> _BytesToNums (byte[] bytes) {
			var bf = new BinaryFormatter();

			using (var ms = new MemoryStream()) {
				ms.Write(bytes, 0, bytes.Length);
				ms.Seek(0, SeekOrigin.Begin);

				var numsBytes = (List<byte[]>)bf.Deserialize(ms);
				return numsBytes.Select(b => OmgNumExtensions.FromByteArray(b));
			}
		}
	}
}
