using System;
using SizeDoesNotMatter;
using Lab2.Algorithms;

namespace Lab2 {
	class Program {
		static void Main (string[] args) {

			// Rho Pollard
			var primeA = "99194853094755497".ToOmgNum();
			var primeB = "2971215073".ToOmgNum();

			var primesProd = OmgOp.Multiply(primeA, primeB);

			var polard = new PolardFactorization();
			//var factor = polard.FindFactor(primesProd);

			// Rho Discrete Log

			var rhoLogFinder = new RhoLog();

			(OmgNum a, OmgNum b, OmgNum p) log = (5.ToOmgNum(), 3.ToOmgNum(), 2017.ToOmgNum());
			var discreteLog = rhoLogFinder.FindLog(log.a, log.b, log.p);

			Console.WriteLine($"{discreteLog} : {OmgOp.Pow(log.a, discreteLog, log.p)}");

		}
	}
}
