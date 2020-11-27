using System;
using System.Collections.Generic;
using System.Text;

namespace SizeDoesNotMatter {
	public class OmgEqSys {

		private LinkedList<Eq> m_system;
		private Dictionary<Eq, LinkedListNode<Eq>> m_lookup;

		private List<(OmgNum root, OmgNum mod)> m_roots;

		private struct Eq {
			public OmgNum A, B, M;

			public Eq( OmgNum a, OmgNum b, OmgNum m ) {
				A = a; B = b; M = m;
			}

			public void Release() {
				A.Release();
				B.Release();
				M.Release();
			}

			public override bool Equals (object obj) {
				if (!(obj is Eq other)) { return false; }
				return A.Equals(other.A) && B.Equals(other.B) && M.Equals(other.M);
			}

			public override int GetHashCode () {
				return A.GetHashCode() ^ B.GetHashCode() ^ M.GetHashCode();
			}

			public override string ToString () {
				return $"{{{A}, {B}, {M}}}";
			}
		}

		public OmgEqSys() {
			m_system = new LinkedList<Eq>();
			m_lookup = new Dictionary<Eq, LinkedListNode<Eq>>();
			m_roots = new List<(OmgNum root, OmgNum mod)>();
		}

		public void AddEq( OmgNum a, OmgNum b, OmgNum m ) {
			OmgNum M = new OmgNum(m);
			M.IsNegative = false;

			_AddAfter(new Eq(OmgOp.Mod(a, M), OmgOp.Mod(b, M), M));
		}

		public bool TrySolve( out List<(OmgNum root, OmgNum mod)> solution ) {
			solution = null;

			_TrySolveIndividuals( onReachingRecursionBottom: () => {
				var dictCopy = new Dictionary<Eq, LinkedListNode<Eq>>();
				var systemCopy = new LinkedList<Eq>();

				foreach( var eq in m_system ) {
					var eqCopy = new Eq(new OmgNum(eq.A), new OmgNum(eq.B), new OmgNum(eq.M));
					var node = systemCopy.AddLast(eqCopy);
					dictCopy.Add(eqCopy, node);
				}

				var oldDict = m_lookup;
				var oldSystem = m_system;

				m_lookup = dictCopy;
				m_system = systemCopy;

				_SeparateNonPrimeModules();
				if (!_Contradictory()) {
					m_roots.Add(_UseChineeseTheoremBaseSolution());
				}

				m_lookup = oldDict;
				m_system = oldSystem;
			} );

			if(m_roots.Count == 0) return false;
			solution = m_roots;

			return true;
		}

		private bool _Contradictory() {
			var mods = new HashSet<OmgNum>();
			foreach( var eq in m_system ) {
				if( mods.Contains(eq.M) ) { return true; }
				mods.Add(eq.M);
			}
			return false;
		}

		private void _TrySolveIndividuals( System.Action onReachingRecursionBottom, bool firstCall = true, LinkedListNode<Eq> begin = null) {
			if (!firstCall && begin == null) {
				var a = m_system;
				onReachingRecursionBottom();
				if (m_system != a) throw new System.Exception();
				return;
			}

			var current = (firstCall) ? m_system.First : begin;

			m_lookup.Remove(current.Value);
			var next = current.Next;

			var initEq = current.Value;
			m_system.Remove(current);

			foreach (var solution in _SolveEq(initEq)) {
				if (!solution.solvable) {
					continue;
				}

				next = (next == null) ? null : m_system.Find(next.Value);
				_AddBefore(solution.eq, next);
				_TrySolveIndividuals(onReachingRecursionBottom, firstCall: false, next);
				_RemoveFromSystem(m_lookup[solution.eq]);
			}

			_AddBefore(initEq, next);
		}

		private IEnumerable<(Eq eq, bool solvable)> _SolveEq( Eq eq ) {
			OmgNum gcd = OmgOp.Gcd(eq.A, eq.M);

			if( gcd.IsOne() ) {
				gcd.Release();
				yield return (_SolveMutualPrimeEq(eq), true);
				yield break;
			}

			var divModRes = OmgOp.DivMod(eq.B, gcd);
			if( !divModRes.mod.IsZero() || gcd.Size > 1 ) {

				gcd.Release();
				divModRes.div.Release();
				divModRes.mod.Release();

				yield return (default, false);
				yield break;
			}

			var dividedM = OmgOp.DivMod(eq.M, gcd);
			var dividedA = OmgOp.DivMod(eq.A, gcd);
			var dividedAModed = OmgOp.Mod(dividedA.div, dividedM.div);
			var dividedBModed = OmgOp.Mod(divModRes.div, dividedM.div);
			
			var dividedEq = new Eq(dividedAModed, dividedBModed, dividedM.div);
			Eq dividedSolution = _SolveMutualPrimeEq(dividedEq);
			
			OmgNum B = dividedSolution.B;
			OmgNum M = dividedSolution.M;

			for( int i = 0; i < gcd.Raw.Digits[0]; i++ ) {
				var mult = OmgOp.Multiply(M, i.ToOmgNum(), eq.M);
				var sln = new Eq(1.ToOmgNum(), OmgOp.Add(B, mult, eq.M), new OmgNum(eq.M));
				mult.Release();

				yield return (sln, true);
			}

			gcd.Release();
			dividedSolution.Release();
			dividedM.mod.Release();
			dividedA.mod.Release();
			dividedA.div.Release();
			divModRes.mod.Release();
			dividedEq.Release();
		}

		private Eq _SolveMutualPrimeEq(Eq eq) {
			if( !OmgOp.TryInverseByMod(eq.A, eq.M, out OmgNum inverse)) {
				throw new OmgFailException();
			}

			OmgNum nwB = OmgOp.Multiply(eq.B, inverse, eq.M);
			return new Eq(1.ToOmgNum(), nwB, new OmgNum(eq.M));
		}

		private void _SeparateNonPrimeModules () {
			var current = m_system.First;

			while(current != null) {
				var target = _FindNonMutualPrime(current, out OmgNum gcd);
				if( target == null ) {
					current = current.Next;
					continue;
				}

				_SplitEq(current, gcd);
				_SplitEq(target, gcd);

				var previous = current;
				current = current.Next;

				_RemoveFromSystem(previous);
				_RemoveFromSystem(target);
				
				gcd.Release();
			}
		}

		private LinkedListNode<Eq> _FindNonMutualPrime(LinkedListNode<Eq> current, out OmgNum gcd) {
			gcd = null;
			OmgNum M = current.Value.M;
			current = current.Next;

			while( current != null ) {
				gcd = OmgOp.Gcd(M, current.Value.M);
				if( !gcd.IsOne() ) {
					return current;
				}

				gcd.Release();
				current = current.Next;
			}

			return null;
		}

		private void _SplitEq ( LinkedListNode<Eq> current, OmgNum factor ) {
			Eq eq = current.Value;

			var divModRes = OmgOp.DivMod(eq.M, factor);
			divModRes.mod.Release();

			Eq splitA = new Eq(OmgOp.Mod(eq.A, factor), OmgOp.Mod(eq.B, factor), new OmgNum(factor));
			Eq splitB = new Eq(OmgOp.Mod(eq.A, divModRes.div), OmgOp.Mod(eq.B, divModRes.div), divModRes.div);

			_AddAfter(splitA, current);
			_AddAfter(splitB, current);
		}

		private (OmgNum x, OmgNum m) _UseChineeseTheoremBaseSolution() {
			OmgNum commonMod = _CalcCommonMod();

			OmgNum solution = 0.ToOmgNum();
			var current = m_system.First;


			while( current != null ) {
				var m = OmgOp.DivMod(commonMod, current.Value.M);
				
				bool success = OmgOp.TryInverseByMod(m.div, current.Value.M, out OmgNum inverse);
				if (!success) { throw new OmgFailException(); }

				var mInverse = OmgOp.Multiply(m.div, inverse, commonMod);
				var solutionMonomial = OmgOp.Multiply(mInverse, current.Value.B, commonMod);

				var nwSolution = OmgOp.Add(solution, solutionMonomial);
				
				m.mod.Release();
				m.div.Release();
				solution.Release();
				mInverse.Release();

				solution = nwSolution;

				current = current.Next;
			}

			var moddedSolution = OmgOp.Mod(solution, commonMod);
			solution.Release();

			return (moddedSolution, commonMod);
		}

		private OmgNum _CalcCommonMod() {
			var current = m_system.First;
			OmgNum res = new OmgNum(current.Value.M);
			current = current.Next;

			while( current != null ) {
				OmgNum nwRes = OmgOp.Multiply(res, current.Value.M);
				res.Release();
				res = nwRes;
				current = current.Next;
			}

			return res;
		}

		private void _AddAfter(Eq eq, LinkedListNode<Eq> previous = null) {
			if (m_lookup.ContainsKey(eq)) { return; }

			var nwEq = (previous == null) ? 
				m_system.AddLast(eq) : 
				m_system.AddAfter(previous, eq);

			m_lookup.Add(eq, nwEq);
		}
		
		private void _AddBefore(Eq eq, LinkedListNode<Eq> next = null) {
			if (m_lookup.ContainsKey(eq)) { return; }

			var nwEq = (next == null || next.List == null) ?
				m_system.AddLast(eq) :
				m_system.AddBefore(next, eq);

			m_lookup.Add(eq, nwEq);
		}

		private void _RemoveFromSystem( LinkedListNode<Eq> eq ) {
			var value = eq.Value;

			m_lookup.Remove(value);
			m_system.Remove(eq);

			value.Release();
		}
	}
}
