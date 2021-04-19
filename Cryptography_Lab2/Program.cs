using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Cryptography_Lab2.RSA;
using SizeDoesNotMatter;

namespace Cryptography_Lab2 {
	class Program {
		private const string c_speedTestFile = "SpeedTest.csv";

		private const string c_testText =
@"Реалізувати власний клас для роботи з RSA. Симулювати обмін текстовими повідомленнями між Алісою та Бобом. 
В реалізації:
Для генерації простих чисел використати алгоритми з 1 лабораторної***
Використовувати функцію Кармайкла замість функції Ейлера.
Використати китайську теорему про залишки для дешифрування***
Порівняти швидкість роботи алгоритму в залежності від бінарної довжини простих чисел.
Повідомлення не тільки шифруються, але й підписуються за допомогою RSA. ***
";

		static void Main (string[] args) {
			//SpeedTest();
			//return;

			ChatingSimulation();
			return;
		}

		private static void SpeedTest () {
			if (File.Exists(c_speedTestFile)) {
				File.Delete(c_speedTestFile);
			}

			List<string> testPoints = new List<string>();
			var sb = new StringBuilder();

			for (int i = 1; i <= 200; i++) {
				sb.Append(System.Guid.NewGuid());
				if (i % 20 == 0) {
					testPoints.Add(sb.ToString());
				}
			}

			var keyGen = new KeyGenerator();
			var keys = new List<KeyGenerator.Key>();
			for (int i = 0; i < 15; i++) {
				int byteLength = 16 + 8 * i;
				keys.Add(keyGen.GenerateKey(byteLength));

				Console.WriteLine($"Generated key with size: {byteLength * 8}");
			}

			string header = "keyBits, mode (enc/dec), ";
			for (int i = 0; i < testPoints.Count; i++) {
				header += $"{testPoints[i].Length} chars, ";
			}
			header = header.Substring(0, header.Length - 2);
			_LogToFile(header);

			var watch = new Stopwatch();

			for (int keyInd = 0; keyInd < keys.Count; keyInd++) {
				int keySize = 8 * (16 + 8 * keyInd);

				var encripted = new List<List<OmgNum>>();
				string row = $"{keySize}, enc, ";

				var converter = new MessageConverter(16 + 8 * keyInd - 1);

				for (int i = 0; i < testPoints.Count; i++) {
					watch.Restart();

					var mesageNums = converter.ToNumbers(testPoints[i]).ToList();
					encripted.Add(RsaUtility.Encode(mesageNums, keys[keyInd].pub).ToList());
					foreach (var n in mesageNums) {
						n.Release();
					}

					watch.Stop();
					row += $"{watch.ElapsedMilliseconds}, ";
				}

				row = row.Substring(0, row.Length - 2);
				_LogToFile(row);

				row = $"{keySize}, dec, ";
				for (int i = 0; i < testPoints.Count; i++) {
					watch.Restart();

					var decodedNums = RsaUtility.Decode(encripted[i], keys[keyInd].priv).ToList();
					string text = converter.FromNumbers(decodedNums);
					foreach (var n in decodedNums) {
						n.Release();
					}

					watch.Stop();
					row += $"{watch.ElapsedMilliseconds}, ";

					if (text != testPoints[i]) {
						Console.WriteLine($"Failed at index {i} with keylength {keySize}");
					}
				}

				row = row.Substring(0, row.Length - 2);
				_LogToFile(row);

				Console.WriteLine($"Tested key {keyInd + 1} / {keys.Count}.");
			}
		}

		private static void _LogToFile (string row) {
			using (var fs = new FileStream(c_speedTestFile, FileMode.Append, FileAccess.Write)) {
				using (var writer = new StreamWriter(fs)) {
					writer.WriteLine(row.Trim());
				}
			}
		}

		private static void ChatingSimulation () {
			var certificateRepo = new CertificateSource();

			var alice = new RsaUser("Alice");
			alice.SetCertificateRepo(certificateRepo);

			var bob = new RsaUser("Bob");
			bob.SetCertificateRepo(certificateRepo);

			alice.SetFriend(bob);
			bob.SetFriend(alice);

			Console.WriteLine();
			alice.SendMessageToFriend("Hello");

			Console.WriteLine();
			bob.SendMessageToFriend("Hi");

			Console.WriteLine();
			alice.SendMessageToFriend(c_testText);

			var notAFriend = new RsaUser("Hacker");
			notAFriend.SetFriend(bob);

			Console.WriteLine();
			notAFriend.SendMessageToFriend("Hello bob it's Alice");
		}
	}
}
