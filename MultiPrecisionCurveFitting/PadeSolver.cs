﻿using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    /// <summary>パデ近似</summary>
    public static class PadeSolver<N> where N : struct, IConstant {
        /// <summary>ソルバー</summary>
        /// <param name="cs">テイラー係数</param>
        /// <param name="m">分子係数</param>
        /// <param name="n">分母係数</param>
        public static (MultiPrecision<N>[] ms, MultiPrecision<N>[] ns) Solve(MultiPrecision<N>[] cs, int m, int n) {
            if (m < 0) {
                throw new ArgumentOutOfRangeException(nameof(m));
            }
            if (n < 0) {
                throw new ArgumentOutOfRangeException(nameof(n));
            }
            if (cs.Length != checked(m + n + 1)) {
                throw new ArgumentException("Illegal length.", nameof(cs));
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

            Vector<N> v = a.Inverse * c;
            MultiPrecision<N>[] ms = new MultiPrecision<N>[] { cs[0] }.Concat(((MultiPrecision<N>[])v)[..m]).ToArray();
            MultiPrecision<N>[] ns = new MultiPrecision<N>[] { 1 }.Concat(((MultiPrecision<N>[])v)[m..]).ToArray();

            return (ms, ns);
        }

        /// <summary>近似</summary>
        /// <param name="a">説明変数</param>
        /// <param name="ms">分子係数</param>
        /// <param name="ns">分母係数</param>
        public static MultiPrecision<N> Approx(MultiPrecision<N> a, MultiPrecision<N>[] ms, MultiPrecision<N>[] ns) {
            MultiPrecision<N> p = ms[^1], q = ns[^1];

            for (int i = ms.Length - 2; i >= 0; i--) {
                p = p * a + ms[i];
            }
            for (int i = ns.Length - 2; i >= 0; i--) {
                q = q * a + ns[i];
            }

            MultiPrecision<N> y = p / q;

            return y;
        }
    }
}
