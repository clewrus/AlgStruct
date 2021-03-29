using System;
using System.Collections.Generic;
using SizeDoesNotMatter;

namespace Lab1 {
	class LabMain {
		static void Main (string[] args) {
			var a = 234234.ToOmgNum();
			var b = "85430534954938530495935803495482304983".ToOmgNum();
			var c = "100000000000000000000000000000000".ToOmgNum();
			var d = "-100000000000000000".ToOmgNum();


			var a1 = OmgOp.Add(a, c);
			var a2 = OmgOp.Subtract(a, c);
			var a3 = OmgOp.Multiply(a, b);
			var a4 = OmgOp.DivMod(b, c);
			var a6 = OmgOp.Pow(b, c, a);
			var a7 = OmgOp.Sqrt(b);


			var sys1 = new OmgEqSys();
			sys1.AddEq(1287.ToOmgNum(), 447.ToOmgNum(), 516.ToOmgNum());
			
			bool success1 = sys1.TrySolve(out List<(OmgNum root, OmgNum mod)> solution1);

			var sys2 = new OmgEqSys();


			bool success2 = sys2.TrySolve(out var solution2);
		}
	}
}
