using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>多項式フィッティング</summary>
    public class PolynomialFitter<N> : Fitter<N> where N : struct, IConstant {
        /// <summary>コンストラクタ</summary>
        public PolynomialFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, int degree, bool enable_intercept)
            : base(xs, ys, checked(degree + (enable_intercept ? 1 : 0))) {

            this.Degree = degree;
            this.EnableIntercept = enable_intercept;
        }

        /// <summary>次数</summary>
        public int Degree {
            get; private set;
        }

        /// <summary>y切片を有効にするか</summary>
        public bool EnableIntercept { get; set; }

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
            Matrix<N> m = new(Points, Parameters);
            Vector<N> b = Vector<N>.Zero(Points);

            if (EnableIntercept) {
                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i];
                    b[i] = Y[i];

                    m[i, 0] = 1;

                    for (int j = 1; j <= Degree; j++) {
                        m[i, j] = m[i, j - 1] * x;
                    }
                }
            }
            else {
                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i];
                    b[i] = Y[i];

                    m[i, 0] = x;

                    for (int j = 1; j < Degree; j++) {
                        m[i, j] = m[i, j - 1] * x;
                    }
                }
            }

            return (m.Transpose * m).Inverse * m.Transpose * b;
        }
    }
}
