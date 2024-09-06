using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    public class PadeFitter<N> : Fitter<N> where N : struct, IConstant {

        private readonly SumTable<N> sum_table;
        private readonly MultiPrecision<N>? intercept;

        /// <summary>分子係数</summary>
        public int Numer { get; private set; }

        /// <summary>分母係数</summary>
        public int Denom { get; private set; }

        /// <summary>コンストラクタ</summary>
        /// <param name="numer">分子係数</param>
        /// <param name="denom">分母係数</param>
        /// <param name="intercept">切片</param>
        public PadeFitter(Vector<N> xs, Vector<N> ys, int numer, int denom, MultiPrecision<N>? intercept = null)
            : base(xs, ys,
                  parameters:
                  (numer >= 2 && denom >= 2)
                      ? (numer + denom)
                      : throw new ArgumentOutOfRangeException($"{nameof(numer)},{nameof(denom)}")) {

            this.sum_table = new(X, Y);
            this.intercept = intercept;
            this.Numer = numer;
            this.Denom = denom;
        }

        public PadeFitter(SumTable<N> sum_table, int numer, int denom, MultiPrecision<N>? intercept = null)
            : base(sum_table.X, sum_table.Y,
                  parameters:
                  (numer >= 2 && denom >= 2)
                      ? (numer + denom)
                      : throw new ArgumentOutOfRangeException($"{nameof(numer)},{nameof(denom)}")) {

            this.sum_table = sum_table;
            this.intercept = intercept;
            this.Numer = numer;
            this.Denom = denom;
        }

        public override MultiPrecision<N> Regress(MultiPrecision<N> x, Vector<N> parameters) {
            if (parameters.Dim != Parameters) {
                throw new ArgumentException("invalid size", nameof(parameters));
            }

            (MultiPrecision<N> numer, MultiPrecision<N> denom) = Fraction(x, parameters);

            MultiPrecision<N> y = numer / denom;

            return y;
        }

        public (MultiPrecision<N> numer, MultiPrecision<N> denom) Fraction(MultiPrecision<N> x, Vector<N> parameters) {
            MultiPrecision<N> n = Vector<N>.Polynomial(x, parameters[..Numer]);
            MultiPrecision<N> d = Vector<N>.Polynomial(x, parameters[Numer..]);

            return (n, d);
        }

        /// <summary>フィッティング</summary>
        public Vector<N> Fit(Vector<N>? weights = null, MultiPrecision<N>? norm_cost = null) {
            sum_table.W = weights;
            (Matrix<N> m, Vector<N> v) = GenerateTable(sum_table, Numer, Denom);

            if (norm_cost is not null) {
                MultiPrecision<N> c = norm_cost * sum_table[0, 0];

                for (int i = 0; i < m.Rows; i++) {
                    m[i, i] += c;
                }
            }

            if (intercept is null) {
                Vector<N> x = Matrix<N>.SolvePositiveSymmetric(m, v, enable_check_symmetric: false);

                Vector<N> parameters = Vector<N>.Concat(x[..Numer], 1, x[Numer..]);

                return parameters;
            }
            else {
                v = v[1..] - intercept * m[0, 1..];
                m = m[1.., 1..];

                Vector<N> x = Matrix<N>.SolvePositiveSymmetric(m, v, enable_check_symmetric: false);

                Vector<N> parameters = Vector<N>.Concat(intercept, x[..(Numer - 1)], 1, x[(Numer - 1)..]);

                return parameters;
            }
        }

        internal static (Matrix<N> m, Vector<N> v) GenerateTable(SumTable<N> sum_table, int numer, int denom) {
            int dim = numer + denom - 1;

            MultiPrecision<N>[,] m = new MultiPrecision<N>[dim, dim];
            for (int i = 0, n = numer; i < n; i++) {
                for (int j = i; j < n; j++) {
                    m[i, j] = m[j, i] = sum_table[i + j, 0];
                }
            }
            for (int i = numer, n = dim; i < n; i++) {
                for (int j = 0; j < numer; j++) {
                    m[i, j] = m[j, i] = -sum_table[i + j - numer + 1, 1];
                }
            }
            for (int i = numer, n = dim; i < n; i++) {
                for (int j = i; j < n; j++) {
                    m[i, j] = m[j, i] = sum_table[i + j - 2 * numer + 2, 2];
                }
            }

            MultiPrecision<N>[] v = new MultiPrecision<N>[numer + denom - 1];
            for (int i = 0; i < numer; i++) {
                v[i] = sum_table[i, 1];
            }
            for (int i = numer; i < dim; i++) {
                v[i] = -sum_table[i - numer + 1, 2];
            }

            return (m, v);
        }
    }
}
