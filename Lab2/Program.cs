using System;
using SizeDoesNotMatter;
using Lab2.Algorithms;
using System.Linq;

namespace Lab2 {
	class Program {
		static void Main (string[] args) {

			// Rho Pollard
			var primeA = "99194853094755497".ToOmgNum();
			var primeB = "2971215073".ToOmgNum();

			var primesProd = OmgOp.Multiply(primeA, primeB);

			var polard = new PolardFactorization();
			var factor = polard.FindFactor(primesProd);
			Console.WriteLine($"factor: {factor}");

			// Rho Discrete Log

			var rhoLogFinder = new RhoLog();

			(OmgNum a, OmgNum b, OmgNum p) log = (5.ToOmgNum(), 3.ToOmgNum(), 2017.ToOmgNum());
			var discreteLog = rhoLogFinder.FindLog(log.a, log.b, log.p);

			Console.WriteLine($"{discreteLog} : {OmgOp.Pow(log.a, discreteLog, log.p)}");

			// Factorization

			var factorizer = new Factorization();
			var factorization = factorizer.Factorize("43212354346700".ToOmgNum());

			Console.WriteLine($"factorization: {String.Join(' ', factorization.Select(x => ($"{x.Key}^{x.Value}")))}");
			// EulerF

			var euler = factorizer.EulerFunction("96".ToOmgNum());
			Console.WriteLine($"euler: {euler}");

			// MobiusF

			var mobius1 = factorizer.Mobius("32809742394".ToOmgNum());
			var mobius2 = factorizer.Mobius("4".ToOmgNum());

			Console.WriteLine($"mobius1: {mobius1}");
			Console.WriteLine($"mobius2: {mobius2}");
		}
	}
}
