using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter.Internal.Operations {
	internal class Rooter {
		OmgNum m_targetSquareNum;
		RawNum m_targetSquare;

		RawNum m_rootAprox;
		RawNum m_buffer;

		internal RawNum Sqrt( RawNum left ) {
			m_targetSquare = left;
			m_targetSquareNum = m_targetSquare.OmgWrapper();

			m_rootAprox = OmgPool.GetRawZero();
			m_buffer = OmgPool.GetRawZero();

			_FillInitialApproximation();

			while( !_AproxEqualToBuffer() ) {
				var aproxCopy = OmgPool.GetRawZero();
				aproxCopy.CopyFrom(m_rootAprox);

				var bufferCopy = OmgPool.GetRawZero();
				bufferCopy.CopyFrom(m_buffer);

				_MakeGeronIteration();

				if( OmgOp.Equal(aproxCopy.OmgWrapper(), m_buffer.OmgWrapper()) &&
					OmgOp.Equal(bufferCopy.OmgWrapper(), m_rootAprox.OmgWrapper()) &&
					OmgOp.Less( m_rootAprox.OmgWrapper(), m_buffer.OmgWrapper())) { 
					break;
				}

				OmgPool.ReleaseNumber(bufferCopy);
				OmgPool.ReleaseNumber(aproxCopy);
			}

			OmgPool.ReleaseNumber(m_buffer);
			return m_rootAprox;
		}

		private void _FillInitialApproximation () {
			int s = m_targetSquare.Size;

			if( s == 1) {
				m_rootAprox.Digits.Add((UInt16)Math.Sqrt(m_targetSquare.Digits[0]));
				return;
			}

			UInt32 head = (m_targetSquare.Digits[s - 1] << 16) | (m_targetSquare.Digits[s - 2]);
			UInt32 approxHead = (UInt32)(Math.Sqrt(head) * ((s % 2 == 0) ? Math.Sqrt(2) : 1));

			m_rootAprox.Digits.Add(approxHead);
			for( int i = 0; i < (s - 1) / 2; i++) {
				m_rootAprox.Digits.Add(0);
			}
		}

		private bool _AproxEqualToBuffer() {
			return OmgOp.Equal(m_rootAprox.OmgWrapper(), m_buffer.OmgWrapper());
		}

		private void _MakeGeronIteration() {
			OmgNum rootAprox = m_rootAprox.OmgWrapper();

			var res = OmgOp.DivMod(m_targetSquareNum, rootAprox);
			res.mod.Release();

			OmgNum sum = OmgOp.Add(rootAprox, res.div);
			res.div.Release();

			sum.Raw.DivByTwo();

			OmgPool.ReleaseNumber(m_buffer);
			m_buffer = m_rootAprox;
			m_rootAprox = sum.Raw;
		}
	}
}
