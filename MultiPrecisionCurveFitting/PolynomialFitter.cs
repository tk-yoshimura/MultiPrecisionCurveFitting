using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>多項式フィッティング</summary>
    public class PolynomialFitter<N> : Fitter<N> where N : struct, IConstant {

        /// <summary>次数</summary>
        public int Degree {
            get; private set;
        }

        /// <summary>y切片を有効にするか</summary>
        public bool EnableIntercept { get; set; }

        /// <summary>コンストラクタ</summary>
        public PolynomialFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, int degree, bool enable_intercept)
            : base(xs, ys, checked(degree + (enable_intercept ? 1 : 0))) {

            this.Degree = degree;
            this.EnableIntercept = enable_intercept;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> coefficients) {
            if (EnableIntercept) {
                MultiPrecision<N> y = coefficients[coefficients.Dim - 1];

                for (int i = coefficients.Dim - 2; i >= 0; i--) {
                    y = y * x + coefficients[i];
                }

                return y;
            }
            else {
                MultiPrecision<N> y = coefficients[coefficients.Dim - 1];

                for (int i = coefficients.Dim - 2; i >= 0; i--) {
                    y = y * x + coefficients[i];
                }
                y *= x;

                return y;
            }
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting() {
            SumTable<N> sum_table = new(new Vector<N>(X), new Vector<N>(Y));
            (Matrix<N> m, Vector<N> v) = GenerateTable(sum_table, Degree, EnableIntercept);

            Vector<N> parameters = Matrix<N>.Solve(m, v);

            return parameters;
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
