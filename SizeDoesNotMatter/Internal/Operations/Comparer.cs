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

			for( int i = left.Size - 1; i >= 0; i-- ) {
				Int32 diff = (Int32)right.Digits[i] - (Int32)left.Digits[i];
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
				if (right.Digits[i] != left.Digits[i]) {
					return false;
				}
			}

			return true;
		}
	}
}
