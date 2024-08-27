using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>多項式フィッティング</summary>
    public class PolynomialFitter<N> : Fitter<N> where N : struct, IConstant {

        private readonly SumTable<N> sum_table;
        private readonly MultiPrecision<N>? intercept;

        /// <summary>次数</summary>
        public int Degree {
            get; private set;
        }

        /// <summary>コンストラクタ</summary>
        public PolynomialFitter(Vector<N> xs, Vector<N> ys, int degree, MultiPrecision<N>? intercept = null)
            : base(xs, ys, parameters: checked(degree + 1)) {

            this.sum_table = new(X, (intercept is null) ? ys : ys.Select(y => y.val - intercept).ToArray());
            this.intercept = intercept;
            this.Degree = degree;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> Regress(MultiPrecision<N> x, Vector<N> parameters) {
            if (parameters.Dim != Parameters) {
                throw new ArgumentException("invalid size", nameof(parameters));
            }

            MultiPrecision<N> y = Vector<N>.Polynomial(x, parameters);

            return y;
        }

        /// <summary>フィッティング</summary>
        public Vector<N> Fit(Vector<N>? weights = null) {
            sum_table.W = weights;
            (Matrix<N> m, Vector<N> v) = GenerateTable(sum_table, Degree, enable_intercept: intercept is null);

            if (intercept is null) {
                Vector<N> parameters = Matrix<N>.Solve(m, v);

                return parameters;
            }
            else {
                Vector<N> parameters = Vector<N>.Concat(intercept, Matrix<N>.Solve(m, v));

                return parameters;
            }
        }

        internal static (Matrix<N> m, Vector<N> v) GenerateTable(SumTable<N> sum_table, int degree, bool enable_intercept) {
            int dim = degree + (enable_intercept ? 1 : 0);

            MultiPrecision<N>[,] m = new MultiPrecision<N>[dim, dim];
            MultiPrecision<N>[] v = new MultiPrecision<N>[dim];

            if (enable_intercept) {
                for (int i = 0; i < dim; i++) {
                    for (int j = i; j < dim; j++) {
                        m[i, j] = m[j, i] = sum_table[i + j, 0];
                    }
                }

                for (int i = 0; i < dim; i++) {
                    v[i] = sum_table[i, 1];
                }
            }
            else {
                for (int i = 0; i < dim; i++) {
                    for (int j = i; j < dim; j++) {
                        m[i, j] = m[j, i] = sum_table[i + j + 2, 0];
                    }
                }

                for (int i = 0; i < dim; i++) {
                    v[i] = sum_table[i + 1, 1];
                }
            }

            return (m, v);
        }
    }
}
