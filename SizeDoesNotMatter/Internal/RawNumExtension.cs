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
			num.RemoveLeadingZeros();
		}

		internal static void MultByTwo (this RawNum num) {
			UInt32 leadingBit = 0;
			for( int i = 0; i < num.Size; i++ ) {
				UInt32 res = num.Digits[i] << 1;

				res |= leadingBit;
				leadingBit = (res >> 16) & 1;

				num.Digits[i] = res & UInt16.MaxValue;
			}

			if( leadingBit > 0 ) {
				num.Digits.Add(leadingBit);
			}
		}

		internal static void Inc (this RawNum num) {
			UInt32 overfit = 1;
			for (int i = 0; i < num.Size && overfit > 0; i++) {
				UInt32 summ = num.Digits[i] + overfit;
				overfit = summ >> 16;
				num.Digits[i] = summ & UInt16.MaxValue;
			}
			if( overfit > 0 ) {
				num.Digits.Add(overfit);
			}
		}

		internal static void Dec (this RawNum num) {
			const UInt32 leadingBit = (1 << 16);
			UInt32 z = 1;

			for (int i = 0; i < num.Size && z > 0; i++) {
				UInt32 sub = (leadingBit | num.Digits[i]) - z;
				num.Digits[i] = sub & UInt16.MaxValue;

				z = (~sub & leadingBit) >> 16;
			}

			num.RemoveLeadingZeros();
		}

		internal static void RemoveLeadingZeros (this RawNum num) {
			while( num.Size > 0 && num.Digits[num.Size - 1] == 0 ) {
				num.Digits.RemoveAt(num.Size - 1);
			}
		}
	}
}
