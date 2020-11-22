﻿using SizeDoesNotMatter.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter {
	public class OmgNum {
		private static BaseConverter<UInt32> m_converter;
		private static StringBuilder m_stringRepresentation;

		internal bool IsNegative { get; set; }
		internal RawNum Raw { get; private set; }
		public bool IsValid => Raw != null;

		static OmgNum() {
			m_converter = new BaseConverter<UInt32>();
			m_converter.InputBase = UInt16.MaxValue + 1;
			m_converter.Output = new List<UInt32>();

			m_stringRepresentation = new StringBuilder();
		}

		internal OmgNum(RawNum raw) {
			this.Raw = raw;
		}

		public void Release() {
			OmgPool.ReleaseNumber(Raw);
			Raw = null;
		}

		public bool IsZero() {
			return Raw.IsZero();
		}

		#region ToString

		public override string ToString () {
			return ToString(10);
		}

		public string ToString(uint _base) {
			m_stringRepresentation.Clear();

			FillStringRepresentation(_base);
			TrimStringRepresentation(_base);
			
			return m_stringRepresentation.ToString();
		}

		private void FillStringRepresentation(uint _base) {
			m_converter.OutputBase = _base;
			m_converter.Input = Digits();
			m_converter.Convert();

			foreach (var digit in m_converter.Output) {
				m_stringRepresentation.Insert(0, DigitToString(digit, _base));
			}

			if (IsNegative) {
				m_stringRepresentation.Insert(0, '-');
			}
		}

		private IEnumerable<UInt32> Digits() {
			for(int i = Raw.Digits.Count - 1; i >= 0; i--) {
				yield return (UInt32)Raw.Digits[i];
			}
		}

		private string DigitToString(UInt32 digit, uint _base) {
			if (_base <= 10) {
				return digit.ToString();
			}

			if (_base <= 10 + ('z' - 'a' + 1)) {
				return (digit < 10) ? digit.ToString() : ((char)('A' + (digit - 10))).ToString();
			}

			return $"{digit.ToString()}, ";
		}

		private void TrimStringRepresentation(uint _base) {
			if (_base > 10 + ('z' - 'a' + 1)) {
				m_stringRepresentation.Remove(m_stringRepresentation.Length - 2, 2);
			}
		}

		#endregion
	}
}
