using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>線形フィッティング</summary>
    public class LinearFitter<N> : Fitter<N> where N : struct, IConstant {

        private readonly SumTable<N> sum_table;
        private readonly MultiPrecision<N>? intercept;

        /// <summary>コンストラクタ</summary>
        public LinearFitter(Vector<N> xs, Vector<N> ys, MultiPrecision<N>? intercept = null)
            : base(xs, ys, parameters: 2) {

            this.sum_table = new(X, (intercept is null) ? ys : ys.Select(y => y.val - intercept).ToArray());
            this.intercept = intercept;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters) {
            if (parameters.Dim != Parameters) {
                throw new ArgumentException("invalid size", nameof(parameters));
            }

            return parameters[0] + parameters[1] * x;
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting(Vector<N>? weights = null) {
            sum_table.W = weights;

            MultiPrecision<N> sum_wxx = sum_table[2, 0], sum_wxy = sum_table[1, 1];

            if (intercept is null) {
                MultiPrecision<N> sum_w = sum_table[0, 0], sum_wx = sum_table[1, 0], sum_wy = sum_table[0, 1];

                MultiPrecision<N> r = 1 / (sum_wx * sum_wx - sum_w * sum_wxx);
                MultiPrecision<N> a = (sum_wx * sum_wxy - sum_wxx * sum_wy) * r;
                MultiPrecision<N> b = (sum_wx * sum_wy - sum_w * sum_wxy) * r;

                return new Vector<N>(a, b);
            }
            else {
                return new Vector<N>(intercept, sum_wxy / sum_wxx);
            }
        }
    }
}
