using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    public class WeightedPadeFitter<N> : Fitter<N> where N : struct, IConstant {

        private readonly MultiPrecision<N>? intercept;

        private readonly IReadOnlyList<MultiPrecision<N>> weights;

        /// <summary>分子係数</summary>
        public int Numer { get; private set; }

        /// <summary>分母係数</summary>
        public int Denom { get; private set; }

        /// <summary>コンストラクタ</summary>
        /// <param name="numer">分子係数</param>
        /// <param name="denom">分母係数</param>
        /// <param name="intercept">切片</param>
        public WeightedPadeFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, IReadOnlyList<MultiPrecision<N>> weights, int numer, int denom, MultiPrecision<N>? intercept = null)
            : base(xs, ys,
                  (numer >= 2 && denom >= 2) ? (numer + denom) : throw new ArgumentOutOfRangeException($"{nameof(numer)},{nameof(denom)}")) {

            this.intercept = intercept;
            this.Numer = numer;
            this.Denom = denom;

            if (Points != weights.Count || !weights.All(w => w.Sign == Sign.Plus)) {
                throw new ArgumentException(null, nameof(weights));
            }

            this.weights = weights;
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
            SumTable<N> sum_table = new(new Vector<N>(X), new Vector<N>(Y), new Vector<N>(weights));
            (Matrix<N> m, Vector<N> v) = PadeFitter<N>.GenerateTable(sum_table, Numer, Denom);

            Vector<N> parameters = Vector<N>.Zero(Numer + Denom);

            if (intercept is null) {
                Vector<N> x = Matrix<N>.Solve(m, v);

                parameters[..Numer] = x[..Numer];
                parameters[Numer] = 1;
                parameters[(Numer + 1)..] = x[Numer..];
            }
            else {
                v = v[1..] - intercept * m[0, 1..];
                m = m[1.., 1..];

                Vector<N> x = Matrix<N>.Solve(m, v);

                parameters[0] = intercept;
                parameters[1..Numer] = x[..(Numer - 1)];
                parameters[Numer] = 1;
                parameters[(Numer + 1)..] = x[(Numer - 1)..];
            }

            return parameters;
        }
    }
}
