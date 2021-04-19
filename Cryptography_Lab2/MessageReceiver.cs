using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography_Lab2.RSA;
using SizeDoesNotMatter;

namespace Cryptography_Lab2 {
	public class MessageReceiver {
		private PrivKey m_decodingKey;
		private MessageConverter m_converter;

		public MessageReceiver (PrivKey decodingKey, MessageConverter converter) {
			m_decodingKey = decodingKey;
			m_converter = converter;
		}

		public string ReceiveMessageFrom (SignCertificate senderCertificate, Message received, out bool signatureIsOk) {
			var messageNums = received.GetMessage().ToList();
			var messageDecodedNums = RsaUtility.Decode(messageNums, m_decodingKey).ToList();

			string message = m_converter.FromNumbers(messageDecodedNums);

			var signatureNums = received.GetSignature();
			signatureIsOk = RsaSignature.SignatureIsValid(message, signatureNums, m_converter, senderCertificate);

			_ReleaseList(messageNums);
			_ReleaseList(messageDecodedNums);
			_ReleaseList(signatureNums);

			return message;
		}

		private void _ReleaseList (IEnumerable<OmgNum> nums) {
			foreach (var num in nums) {
				num.Release();
			}
		}
	}
}
