using SizeDoesNotMatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2.Algorithms {
	public class RhoLog {
		private OmgNum m_mod;
		private OmgNum m_modMinusOne;

		private (OmgNum third, OmgNum twoThird) m_split;

		private (OmgNum z, OmgNum u, OmgNum v) m_turtle;
		private (OmgNum z, OmgNum u, OmgNum v) m_rabit;

		private (OmgNum a, OmgNum b) m_log;

		public OmgNum FindLog( OmgNum a, OmgNum b, OmgNum mod ) {
			m_mod = mod;
			m_log = (a, b);

			_InitializeSequence();

			OmgNum res = null;
			while (res == null) {
				do {
					_MakeIteration();
				} while (!OmgOp.Equal(m_turtle.z, m_rabit.z) || OmgOp.Equal(m_turtle.u, m_rabit.u));

				res = _EvaluateFinalResult();
			}

			return res;
		}

		private void _InitializeSequence() {
			m_modMinusOne = OmgOp.Subtract(m_mod, 1.ToOmgNum());

			m_turtle = (1.ToOmgNum(), 0.ToOmgNum(), 0.ToOmgNum());
			m_rabit = (1.ToOmgNum(), 0.ToOmgNum(), 0.ToOmgNum());

			m_split.twoThird = OmgOp.DivMod(m_mod.Copy().MultByTwo(), 3.ToOmgNum()).div;
			m_split.third = m_split.twoThird.Copy().DivByTwo();
		}

		private void _MakeIteration() {
			_PropagateSequence(ref m_rabit);
			_PropagateSequence(ref m_rabit);

			_PropagateSequence(ref m_turtle);
		}

		private void _PropagateSequence( ref (OmgNum z, OmgNum u, OmgNum v) seq ) {
			OmgNum nwZ;

			if( OmgOp.Less(seq.z, m_split.third) ) {
				seq.u.Inc();
				nwZ = OmgOp.Multiply(m_log.a, seq.z, m_mod);
			}
			else if( OmgOp.Less( seq.z, m_split.twoThird ) ) {
				seq.u.MultByTwo();
				seq.v.MultByTwo();
				nwZ = OmgOp.Multiply(seq.z, seq.z, m_mod);
			} else {
				seq.v.Inc();
				nwZ = OmgOp.Multiply(m_log.b, seq.z, m_mod);
			}

			_SetModdedValue(ref seq.u, m_modMinusOne);
			_SetModdedValue(ref seq.v, m_modMinusOne);

			seq.z.Release();
			seq.z = nwZ;
		}

		private void _SetModdedValue( ref OmgNum target, OmgNum mod) {
			OmgNum nw = OmgOp.Mod(target, mod);
			target.Release();
			target = nw;
		}

		private OmgNum _EvaluateFinalResult() {
			OmgNum udif = OmgOp.Subtract(m_rabit.u, m_turtle.u, m_modMinusOne);
			OmgNum vdif = OmgOp.Subtract(m_turtle.v, m_rabit.v, m_modMinusOne);


			OmgEqSys sys = new OmgEqSys();
			sys.AddEq(vdif, udif, m_modMinusOne);

			if( sys.TrySolve(out var solution) ) {
				foreach (var s in solution ) {
					var check = OmgOp.Pow(m_log.a, s.root, m_mod);
					if( OmgOp.Equal(check, m_log.b) ) {
						return s.root;
					}

					check.Release();
					s.root.Release();
					s.mod.Release();
				}
			}

			return null;
		}
	}
}
