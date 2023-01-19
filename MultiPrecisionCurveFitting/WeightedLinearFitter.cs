using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>重み付き線形フィッティング</summary>
    public class WeightedLinearFitter<N> : Fitter<N> where N : struct, IConstant {

        readonly IReadOnlyList<MultiPrecision<N>> weights;

        /// <summary>コンストラクタ</summary>
        public WeightedLinearFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, IReadOnlyList<MultiPrecision<N>> weights, bool enable_intercept)
            : base(xs, ys, enable_intercept ? 2 : 1) {

            EnableIntercept = enable_intercept;

            if (weights is null) {
                throw new ArgumentNullException(nameof(weights));
            }

            if (Points != weights.Count) {
                throw new ArgumentException(null, $"{nameof(weights)}");
            }

            foreach (var weight in weights) {
                if (!(weight >= 0)) {
                    throw new ArgumentException(null, nameof(weights));
                }
            }

            this.weights = weights;
        }

        /// <summary>y切片を有効にするか</summary>
        public bool EnableIntercept { get; private set; }

        /// <summary>重み付き誤差二乗和</summary>
        public MultiPrecision<N> WeightedCost(Vector<N> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Dim != Parameters) {
                throw new ArgumentException(null, nameof(parameters));
            }

            Vector<N> errors = Error(parameters);
            MultiPrecision<N> cost = 0;
            for (int i = 0; i < errors.Dim; i++) {
                cost += weights[i] * errors[i] * errors[i];
            }

            return cost;
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
            if (weights is null) {
                throw new InvalidOperationException();
            }

            if (EnableIntercept) {
                MultiPrecision<N> w, sum_w = 0, sum_wx = 0, sum_wy = 0, sum_wxx = 0, sum_wxy = 0;

                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i], y = Y[i];

                    w = weights[i];
                    sum_w += w;
                    sum_wx += w * x;
                    sum_wy += w * y;
                    sum_wxx += w * x * x;
                    sum_wxy += w * x * y;
                }

                MultiPrecision<N> r = 1 / (sum_wx * sum_wx - sum_w * sum_wxx);
                MultiPrecision<N> a = (sum_wx * sum_wxy - sum_wxx * sum_wy) * r;
                MultiPrecision<N> b = (sum_wx * sum_wy - sum_w * sum_wxy) * r;

                return new Vector<N>(a, b);
            }
            else {
                MultiPrecision<N> w, sum_wxx = 0, sum_wxy = 0;

                for (int i = 0; i < Points; i++) {
                    MultiPrecision<N> x = X[i], y = Y[i];

                    w = weights[i];
                    sum_wxx += w * x * x;
                    sum_wxy += w * x * y;
                }

                return new Vector<N>(sum_wxy / sum_wxx);
            }
        }
    }
}
