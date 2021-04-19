using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SizeDoesNotMatter;

namespace Cryptography_Lab2.RSA {
	public class MessageConverter {


		private int m_bytesPerNumber;
		private List<byte> m_bytes;

		byte[] m_numBytes;

		public MessageConverter (int bytesPerNumber) {
			m_bytesPerNumber = bytesPerNumber;
			m_bytes = new List<byte>();

			m_numBytes = new byte[m_bytesPerNumber + 1];
			m_numBytes[m_bytesPerNumber] = 0;
		}

		public IEnumerable<OmgNum> ToNumbers (string input) {
			byte[] message = UnicodeEncoding.UTF8.GetBytes(input);

			for (int i = 0; i < message.Length / m_bytesPerNumber + 1; i++) {
				int start = m_bytesPerNumber * i;
				int length = Math.Min(m_bytesPerNumber, message.Length - start);
				Array.Copy(message, start, m_numBytes, 0, length);

				if (length < m_bytesPerNumber) {
					Array.Fill<byte>(m_numBytes, byte.MaxValue, startIndex: length, count: m_bytesPerNumber - length);
					yield return OmgNumExtensions.FromByteArray(m_numBytes);

					Array.Fill<byte>(m_numBytes, 0, startIndex: 0, count: m_bytesPerNumber - length);
					Array.Fill<byte>(m_numBytes, byte.MaxValue, startIndex: m_bytesPerNumber - length, count: length);
					yield return OmgNumExtensions.FromByteArray(m_numBytes);
				} else {
					yield return OmgNumExtensions.FromByteArray(m_numBytes);
				}
			}
		}

		public string FromNumbers (IEnumerable<OmgNum> numbers) {
			_ConvertNumbersToBytes(numbers);
			_RemoveEndingBytes();

			return UnicodeEncoding.UTF8.GetString(m_bytes.ToArray());
		}

		private void _ConvertNumbersToBytes (IEnumerable<OmgNum> numbers) {
			m_bytes.Clear();

			foreach (OmgNum num in numbers) {
				byte[] numBytes = num.ToByteArray();

				if (numBytes.Length > m_bytesPerNumber + 1) {
					throw new OmgFailException("Too big number was met");
				}

				for (int i = 0; i < m_bytesPerNumber - numBytes.Length + 1; i++) {
					m_bytes.Add(0);
				}

				m_bytes.AddRange(numBytes.SkipLast(1));
			}
		}

		private void _RemoveEndingBytes () {
			int paddedBytes = 0;
			bool zeroPrefixStart = false;

			for (int i = 0; i < m_bytesPerNumber; i++) {
				byte curByte = m_bytes[m_bytes.Count - i - 1];

				if (!zeroPrefixStart && curByte == byte.MaxValue) {
					paddedBytes += 1;
					continue;
				}

				zeroPrefixStart = true;

				if (curByte != 0) {
					throw new OmgFailException("Wrong number sequence");
				}
			}

			int bytesToRemove = 2 * m_bytesPerNumber - paddedBytes;
			m_bytes.RemoveRange(m_bytes.Count - bytesToRemove, bytesToRemove);
		}
	}
}
