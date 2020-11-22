using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter {
	public class OmgFailException : Exception {

		public OmgFailException () : base() { }

		public OmgFailException (string message) : base(message) { }
	}
}
