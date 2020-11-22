using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal {
	internal class BaseConverter<T> where T : struct, IConvertible {
		internal UInt32 InputBase { get; set; }
		internal UInt32 OutputBase { get; set; }

		private List<UInt32> m_digitsA;
		private List<UInt32> m_digitsB;

		internal IEnumerable<UInt32> Input { get; set; }
		internal List<T> Output { get; set; }

		internal BaseConverter() {
			m_digitsA = new List<UInt32>();
			m_digitsB = new List<UInt32>();
		}

		internal void Convert() {
			if (Input == null || Output == null) { return; }

			Output.Clear();
			foreach (var outputDigit in BreakOnRawDigits()) {
				Output.Add(outputDigit);
			}
		}

		private IEnumerable<T> BreakOnRawDigits () {
			ReadNumberDigits(m_digitsA);

			while (m_digitsA.Count > 0) {
				m_digitsB.Clear();
				yield return FindDivMod();

				SwapBuffers();
			}
		}

		private T FindDivMod () {
			UInt32 buffer = 0;
			int pointer = 0;

			while (pointer < m_digitsA.Count) {
				buffer *= InputBase;
				buffer += m_digitsA[pointer++];

				if (m_digitsB.Count > 0 || buffer >= OutputBase) {
					m_digitsB.Add(buffer / OutputBase);
					buffer %= OutputBase;
				}
			}

			return (T)(dynamic)buffer;
		}

		private void SwapBuffers () {
			var temp = m_digitsB;
			m_digitsB = m_digitsA;
			m_digitsA = temp;
		}

		private void ReadNumberDigits (List<UInt32> buffer) {
			buffer.Clear();
			foreach(var digit in Input) {

				if (digit >= InputBase) {
					throw new OmgFailException($"Digit({digit}) can't be greater than base({InputBase})");
				}

				buffer.Add(digit);
			}
		}

	}
}
