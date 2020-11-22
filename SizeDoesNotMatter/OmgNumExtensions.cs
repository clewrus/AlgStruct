using SizeDoesNotMatter.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter {
	public static class OmgNumExtensions {
		private static BaseConverter<UInt32> m_converter;

		static OmgNumExtensions() {
			m_converter = new BaseConverter<UInt32>();
			m_converter.OutputBase = UInt16.MaxValue + 1;
		}

		public static OmgNum ToOmgNum(this int number) {
			OmgNum result = OmgPool.GetZero();
			RawNum rawNumber = result.Raw;

			result.IsNegative = number < 0;

			UInt32 absNum = (UInt32)Math.Abs(number);
			rawNumber.Digits.Add((UInt16)(absNum & ((1 << 16) - 1)));
			rawNumber.Digits.Add((UInt16)(absNum >> 16));

			return result;
		}

		public static OmgNum ToOmgNum(this string strNumber, uint _base = 10) {
			OmgNum result = OmgPool.GetZero();
			RawNum rawNumber = result.Raw;

			result.IsNegative = StartsWithMinus(strNumber);

			m_converter.InputBase = _base;
			m_converter.Input = ParseDigits(strNumber);

			m_converter.Output = rawNumber.Digits;
			m_converter.Convert();

			return result;
		}

		#region Private

		private static bool StartsWithMinus(string str) {
			int pointer = 0;
			while(pointer < str.Length && Char.IsWhiteSpace(str[pointer])) {
				pointer++;
			}

			return str[pointer] == '-';
		}

		private static IEnumerable<UInt32> ParseDigits(string number) {
			foreach (var symbol in number) {
				if (Char.IsLetterOrDigit(symbol)) {
					yield return ParseDigit(Char.ToLower(symbol));
				}
			}
		}

		private static UInt32 ParseDigit(char symbol) {
			if (Char.IsDigit(symbol)) {
				return (UInt32)symbol - '0';
			} else if (Char.IsLetter(symbol)) {
				return (UInt32)symbol - 'a' + 10;
			} else {
				throw new OmgFailException($"Unexpected symbol in number.");
			}
		}




		#endregion
	}
}
