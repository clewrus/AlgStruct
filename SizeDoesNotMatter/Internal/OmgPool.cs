using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal {
	internal class OmgPool {
		private static Stack<RawNum> m_availableNumbers = new Stack<RawNum>();

		internal static OmgNum GetZero() {
			var raw = GetRawZero();
			return new OmgNum(raw);
		}

		internal static RawNum GetRawZero() {
			return (m_availableNumbers.Count == 0) ? new RawNum() : m_availableNumbers.Pop();
		}

		internal static void ReleaseNumber( RawNum number ) {
			if( number == null ) { return; }
			number.Digits.Clear();
			m_availableNumbers.Push(number);
		}
	}
}
