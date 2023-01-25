using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    public class PadeFitter<N> : Fitter<N> where N : struct, IConstant {
        /// <summary>y切片</summary>
        public MultiPrecision<N>? Intercept { get; private set; }

        /// <summary>分子係数</summary>
        public int Numer { get; private set; }

        /// <summary>分母係数</summary>
        public int Denom { get; private set; }

        /// <summary>コンストラクタ</summary>
        /// <param name="numer">分子係数</param>
        /// <param name="denom">分母係数</param>
        /// <param name="intercept">切片</param>
        public PadeFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, int numer, int denom, MultiPrecision<N>? intercept = null)
            : base(xs, ys,
                  (numer >= 2 && denom >= 2) ? (numer + denom) : throw new ArgumentOutOfRangeException($"{nameof(numer)},{nameof(denom)}")) {

            this.Intercept = intercept;
            this.Numer = numer;
            this.Denom = denom;
        }

        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters) {
            (MultiPrecision<N> numer, MultiPrecision<N> denom) = Fraction(x, parameters);

            MultiPrecision<N> y = numer / denom;

            return y;
        }

        protected static MultiPrecision<N> Polynomial(MultiPrecision<N> x, MultiPrecision<N>[] coefs) {
            MultiPrecision<N> y = coefs[^1];

            for (int i = coefs.Length - 2; i >= 0; i--) {
                y = x * y + coefs[i];
            }

            return y;
        }

        public (MultiPrecision<N> numer, MultiPrecision<N> denom) Fraction(MultiPrecision<N> x, Vector<N> parameters) {
            MultiPrecision<N> n = Polynomial(x, parameters[..Numer]);
            MultiPrecision<N> d = Polynomial(x, parameters[Numer..]);

            return (n, d);
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting() {
            SumTable<N> sum_table = new(X.ToArray(), Y.ToArray());

            Matrix<N> m = Matrix<N>.Zero(Numer + Denom - 1, Numer + Denom - 1);
            for (int i = 0, n = Numer; i < n; i++) {
                for (int j = i; j < n; j++) {
                    m[i, j] = m[j, i] = sum_table[i + j, 0];
                }
            }
            for (int i = Numer, n = m.Rows; i < n; i++) {
                for (int j = 0; j < Numer; j++) {
                    m[i, j] = m[j, i] = -sum_table[i + j - Numer + 1, 1];
                }
            }
            for (int i = Numer, n = m.Rows; i < n; i++) {
                for (int j = i; j < n; j++) {
                    m[i, j] = m[j, i] = sum_table[i + j - 2 * Numer + 2, 2];
                }
            }

            Vector<N> v = Vector<N>.Zero(Numer + Denom - 1);
            for (int i = 0; i < Numer; i++) {
                v[i] = sum_table[i, 1];
            }
            for (int i = Numer; i < v.Dim; i++) {
                v[i] = -sum_table[i - Numer + 1, 2];
            }

            if (Intercept is null) {
                Vector<N> x = m.Inverse * v;

                Vector<N> parameters = Vector<N>.Zero(Numer + Denom);
                parameters[..Numer] = x[..Numer];
                parameters[Numer] = 1;
                parameters[(Numer + 1)..] = x[Numer..];

                return parameters;
            }
            else {
                v = v[1..] - Intercept * m[0, 1..];
                m = m[1.., 1..];

                Vector<N> x = m.Inverse * v;

                Vector<N> parameters = Vector<N>.Zero(Numer + Denom);
                parameters[0] = Intercept;
                parameters[1..Numer] = x[..(Numer - 1)];
                parameters[Numer] = 1;
                parameters[(Numer + 1)..] = x[(Numer - 1)..];

                return parameters;
            }
        }
    }
}
