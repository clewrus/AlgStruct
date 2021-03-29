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
			if( number == 0 ) {
				return result;
			}

			RawNum rawNumber = result.Raw;
			result.IsNegative = number < 0;

			UInt32 absNum = (UInt32)Math.Abs(number);
			rawNumber.Digits.Add((UInt16)(absNum & ((1 << 16) - 1)));

			if((absNum >> 16) > 0) {
				rawNumber.Digits.Add((UInt16)(absNum >> 16));
			}

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

		public static OmgNum Copy(this OmgNum num) {
			var copy = new OmgNum(num);
			return copy;
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

		public static byte[] ToByteArray (this OmgNum num) {
			if (num.IsZero()) {
				return new byte[1];
			}

			var byteList = new List<byte>();
			for (int i = 0; i < num.Size; i++) {
				UInt32 digit = num.Raw.Digits[i];
				byteList.Add((byte)(digit & ((1 << 8) - 1)));
				digit >>= 8;
				byteList.Add((byte)(digit & ((1 << 8) - 1)));
			}

			while (byteList.Count > 0 && byteList[byteList.Count - 1] == 0) {
				byteList.RemoveAt(byteList.Count - 1);
			}

			byteList.Add((num.IsNegative) ? (byte)1 : (byte)0);
			return byteList.ToArray();
		}

		public static string EncodeToBase64 (this OmgNum num) {
			var bytes = num.ToByteArray();
			return Convert.ToBase64String(bytes);
		}

		public static OmgNum DecodeFromBase64( this string strNum ) {
			byte[] bytes = Convert.FromBase64String(strNum);

			RawNum num = OmgPool.GetRawZero();

			for( int i = 0; i < bytes.Length - 1; i += 2 ) {
				byte least = bytes[i];
				byte elder = (i + 1 < bytes.Length - 1) ? bytes[i + 1] : (byte)0;
				num.Digits.Add((UInt32)(least | elder << 8));
			}

			OmgNum decoded = new OmgNum(num);
			decoded.IsNegative = bytes[bytes.Length - 1] > 0;

			return decoded;
		}

		#endregion
	}
}
