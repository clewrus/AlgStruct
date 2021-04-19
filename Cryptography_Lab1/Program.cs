using SizeDoesNotMatter;
using SizeDoesNotMatter.PrimeTesting;
using System;

namespace Cryptography_Lab1 {
	class Program {
		public static void Main( string[] args ) {
			var primeGenerator = new PrimeGenerator();

			OmgNum[] primes = new OmgNum[5];
			for( int i = 0; i < primes.Length; i++ ) {
				primes[i] = primeGenerator.GeneratePrime(128);
				Console.WriteLine($"prime {i}: {primes[i]}");
			}

			Console.WriteLine();

			Console.WriteLine($"base2 : {primes[0].ToString(2)}");
			Console.WriteLine($"base10: {primes[0].ToString(10)}");
			Console.WriteLine($"base64: {primes[0].ToString(64)}");

			Console.WriteLine($"base64: {primes[0].EncodeToBase64()}");
		}
	}
}
