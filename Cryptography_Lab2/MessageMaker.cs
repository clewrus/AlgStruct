using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cryptography_Lab2.RSA;
using SizeDoesNotMatter;

namespace Cryptography_Lab2 {
	public class MessageMaker {
		private SignSecret m_sigSecret;
		private MessageConverter m_converter;

		public MessageMaker (SignSecret signatureSecret, MessageConverter converter) {
			m_sigSecret = signatureSecret;
			m_converter = converter;
		}

		public Message FormMessageFor (PubKey receiversKey, string message) {
			var messageNums = m_converter.ToNumbers(message).ToList();

			var encoded = RsaUtility.Encode(messageNums, receiversKey).ToList();
			var signature = RsaSignature.MakeSignature(message, m_converter, m_sigSecret).ToList();

			var m = Message.FromNumbers(encoded, signature);

			_ReleaseList(messageNums);
			_ReleaseList(encoded);
			_ReleaseList(signature);

			return m;
		}

		private void _ReleaseList (IEnumerable<OmgNum> nums) {
			foreach (var num in nums) {
				num.Release();
			}
		}
	}
}
