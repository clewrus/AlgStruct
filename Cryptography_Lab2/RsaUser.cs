using System;
using System.Collections.Generic;
using System.Text;
using Cryptography_Lab2.RSA;

namespace Cryptography_Lab2 {
	public class RsaUser {
		private const int c_keySize = 16;

		public string UserName { get; private set; }

		private KeyGenerator.Key m_communicationKey;
		private KeyGenerator.SignaureKey m_signatureKey;

		private MessageMaker m_messageMaker;
		private MessageReceiver m_messageReceiver;

		private CertificateSource m_sertSource;

		public RsaUser Friend { get; private set; }

		public RsaUser (string name) {
			UserName = name;

			_GenerateKeys();
			_FinishInitialization();
		}

		public override string ToString () {
			return UserName;
		}

		public void SetFriend (RsaUser friend) {
			Friend = friend;
			_Log($"Has new fiend {friend.UserName}");
		}

		public PubKey GetKey () {
			_Log($"Smbdy requested my public key");
			return m_communicationKey.pub;
		}

		public void SetCertificateRepo (CertificateSource source) {
			m_sertSource = source;
			_Log("Register in new certificate repo");
			m_sertSource.AddUser(this, m_signatureKey.cert);
		}

		public void ReceiveMessage (Message received) {
			var friendsCert = m_sertSource.RequestCertificate(Friend);
			string message = m_messageReceiver.ReceiveMessageFrom(friendsCert, received, out bool sertIsOk);
			_Log((sertIsOk) ? $"Got a message from my friend {Friend}. Signature is ok." : "Message from noname");
			_Log($"Received: {message}");
		}

		public void SendMessageToFriend (string text) {
			_Log($"Going to send message to {Friend}");

			var friendsKey = Friend.GetKey();
			Message m = m_messageMaker.FormMessageFor(friendsKey, text);

			Friend.ReceiveMessage(m);
		}

		private void _GenerateKeys () {
			_Log("Start generating keys");

			var generator = new KeyGenerator();

			m_communicationKey = generator.GenerateKey(c_keySize);
			_Log("Generated keys for communication");

			m_signatureKey = generator.GenerateSignature(c_keySize);
			_Log("Generated signiture keys");
		}

		private void _FinishInitialization () {
			var converter = new MessageConverter(c_keySize - 1);
			m_messageMaker = new MessageMaker(m_signatureKey.secret, converter);
			m_messageReceiver = new MessageReceiver(m_communicationKey.priv, converter);
		}

		private void _Log (string s) {
			Console.WriteLine($"{UserName}: {s}");
		}
	}
}
