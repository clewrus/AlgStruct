using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal {
	internal class RawNum {
		internal List<UInt32> Digits { get; set; }
		internal int Size => Digits.Count;

		internal RawNum() {
			Digits = new List<UInt32>();
		}

		internal bool IsZero() {
			return Size == 0;
		}

		internal void Clear() {
			Digits.Clear();
		}

		public override string ToString () {
			return (new OmgNum(this)).ToString();
		}

		internal void CopyFrom( RawNum other ) {
			this.Digits.Clear();
			this.Digits.AddRange(other.Digits);
		}
	}
}
