using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Comparer {


		public bool Less( RawNum left, RawNum right ) {
			int sizeDif = right.Size - left.Size;
			if( sizeDif != 0 ) {
				return sizeDif > 0;
			}

			for( int i = 0; i < left.Size; i++ ) {
				UInt32 diff = right.Digits[i] - left.Digits[i];
				if( diff != 0 ) {
					return diff > 0;
				}
			}

			return false;
		}

		public bool Equal( RawNum left, RawNum right ) {
			int sizeDif = right.Size - left.Size;
			if (sizeDif != 0) {
				return false;
			}

			for (int i = 0; i < left.Size; i++) {
				UInt32 diff = right.Digits[i] - left.Digits[i];
				if (diff != 0) {
					return false;
				}
			}

			return true;
		}
	}
}
