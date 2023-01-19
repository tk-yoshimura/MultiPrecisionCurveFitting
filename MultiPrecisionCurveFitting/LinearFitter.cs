using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>線形フィッティング</summary>
    public class LinearFitter<N> : Fitter<N> where N : struct, IConstant {
        /// <summary>y切片を有効にするか</summary>
        public bool EnableIntercept { get; private set; }

        /// <summary>コンストラクタ</summary>
        public LinearFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, bool enable_intercept)
            : base(xs, ys, enable_intercept ? 2 : 1) {

            EnableIntercept = enable_intercept;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Dim != Parameters) {
                throw new ArgumentException(null, nameof(parameters));
            }

            if (EnableIntercept) {
                return parameters[0] + parameters[1] * x;
            }
            else {
                return parameters[0] * x;
            }
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting() {
            if (EnableIntercept) {
                MultiPrecision<N> sum_x = 0, sum_y = 0, sum_sq_x = 0, sum_xy = 0, n = Points;

                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i], y = Y[i];

                    sum_x += x;
                    sum_y += y;
                    sum_sq_x += x * x;
                    sum_xy += x * y;
                }

                MultiPrecision<N> r = 1 / (sum_x * sum_x - n * sum_sq_x);
                MultiPrecision<N> a = (sum_x * sum_xy - sum_sq_x * sum_y) * r;
                MultiPrecision<N> b = (sum_x * sum_y - n * sum_xy) * r;

                return new Vector<N>(a, b);
            }
            else {
                MultiPrecision<N> sum_sq_x = 0, sum_xy = 0;

                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i], y = Y[i];

                    sum_sq_x += x * x;
                    sum_xy += x * y;
                }

                return new Vector<N>(sum_xy / sum_sq_x);
            }
        }
    }
}
