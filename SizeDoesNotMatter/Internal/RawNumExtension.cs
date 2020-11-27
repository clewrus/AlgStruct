using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal {
	internal static class RawNumExtension {

		internal static OmgNum OmgWrapper(this RawNum num) {
			return new OmgNum(num);
		}

		internal static void DivByTwo (this RawNum num) {
			UInt32 leadingBit = 0;
			for (int i = num.Size - 1; i >= 0; i--) {
				UInt32 bitVal = num.Digits[i];
				num.Digits[i] = (leadingBit << 15) | (bitVal >> 1);
				leadingBit = bitVal & 1;
			}
			if (num.Digits[num.Size - 1] == 0) {
				num.Digits.RemoveAt(num.Size - 1);
			}
		}
	}
}
