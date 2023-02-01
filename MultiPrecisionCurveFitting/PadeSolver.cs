using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    /// <summary>パデ近似</summary>
    public static class PadeSolver<N> where N : struct, IConstant {
        /// <summary>ソルバー</summary>
        /// <param name="cs">テイラー係数</param>
        /// <param name="m">分子係数</param>
        /// <param name="n">分母係数</param>
        public static (Vector<N> ms, Vector<N> ns) Solve(Vector<N> cs, int m, int n) {
            if (m < 0) {
                throw new ArgumentOutOfRangeException(nameof(m));
            }
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n));
            }
            if (cs.Dim != checked(m + n + 1)) {
                throw new ArgumentException("invalid size", nameof(cs));
            }

            int k = m + n;

            Matrix<N> a = Matrix<N>.Zero(k, k);
            Vector<N> c = cs[1..];

            for (int i = 0; i < m; i++) {
                a[i, i] = 1;
            }

            for (int i = m; i < k; i++) {
                for (int j = i - m, r = 0; j < k; j++, r++) {
                    a[j, i] = -cs[r];
                }
            }

            Vector<N> v = Matrix<N>.Solve(a, c);
            Vector<N> ms = Vector<N>.Concat(cs[0], v[..m]), ns = Vector<N>.Concat(1, v[m..]);

            return (ms, ns);
        }
    }
}
