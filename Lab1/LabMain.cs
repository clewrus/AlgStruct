using System;
using SizeDoesNotMatter;

namespace Lab1 {
	class LabMain {
		static void Main (string[] args) {
			string num = "-79A446FB9A604B7B1E5F50FB0B6ED44DD6EDEBD60";
			var n = num.ToOmgNum( 16 );

			var a = "37495837598347384957934857984537".ToOmgNum();
			var b = "10000000000000000000000000000000000000000000000000000".ToOmgNum();

			var c = OmgOp.DivMod(b, a);

			Console.Out.WriteLine(c);
		}
	}
}
