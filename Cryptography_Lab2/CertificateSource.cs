using System;
using System.Collections.Generic;
using System.Text;
using Cryptography_Lab2.RSA;

namespace Cryptography_Lab2 {
	public class CertificateSource {
		private Dictionary<RsaUser, SignCertificate> m_certificates;

		public CertificateSource() {
			m_certificates = new Dictionary<RsaUser, SignCertificate>();
		}

		public void AddUser( RsaUser user, SignCertificate certificate ) {
			Console.WriteLine($"Register certificate for {user}");
			m_certificates[user] = certificate;
		}

		public SignCertificate RequestCertificate ( RsaUser targetUser ) {
			Console.WriteLine($"Certificate for user {targetUser} was requested");
			return m_certificates[targetUser];
		}
	}
}
