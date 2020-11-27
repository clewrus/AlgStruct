using System;
using System.Collections.Generic;
using SizeDoesNotMatter;

namespace Lab1 {
	class LabMain {
		static void Main (string[] args) {
			var sys = new OmgEqSys();
			sys.AddEq(1287.ToOmgNum(), 447.ToOmgNum(), 516.ToOmgNum());
			sys.AddEq(1.ToOmgNum(), 0.ToOmgNum(), 3.ToOmgNum());

			bool success = sys.TrySolve(out List<(OmgNum root, OmgNum mod)> solution);
		}
	}
}
