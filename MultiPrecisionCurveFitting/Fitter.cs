using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    /// <summary>フィッティング基本クラス</summary>
    public abstract class Fitter<N> where N : struct, IConstant {

        /// <summary>フィッティング対象の独立変数</summary>
        public IReadOnlyList<MultiPrecision<N>> X { get; private set; }

        /// <summary>フィッティング対象の従属変数</summary>
        public IReadOnlyList<MultiPrecision<N>> Y { get; private set; }

        /// <summary>フィッティング対象数</summary>
        public int Points { get; private set; }

        /// <summary>パラメータ数</summary>
        public int Parameters { get; private set; }

        /// <summary>コンストラクタ</summary>
        public Fitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, int parameters) {
            if (xs is null) {
                throw new ArgumentNullException(nameof(xs));
            }
            if (ys is null) {
                throw new ArgumentNullException(nameof(ys));
            }
            if (xs.Count < parameters || xs.Count != ys.Count) {
                throw new ArgumentException($"{nameof(xs.Count)}, {nameof(ys.Count)}");
            }
            if (parameters < 1) {
                throw new ArgumentException(null, nameof(parameters));
            }

            this.X = xs;
            this.Y = ys;
            this.Points = xs.Count;
            this.Parameters = parameters;
        }

        /// <summary>誤差二乗和</summary>
        public MultiPrecision<N> Cost(Vector<N> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Dim != Parameters) {
                throw new ArgumentException(null, nameof(parameters));
            }

            Vector<N> errors = Error(parameters);
            MultiPrecision<N> cost = 0;
            for (int i = 0; i < errors.Dim; i++) {
                cost += errors[i] * errors[i];
            }

            return cost;
        }

        /// <summary>誤差</summary>
        public Vector<N> Error(Vector<N> parameters) {
            if (parameters is null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (parameters.Dim != Parameters) {
                throw new ArgumentException(null, nameof(parameters));
            }

            Vector<N> errors = Vector<N>.Zero(Points);

            Vector<N> approxs = Vector<N>.Zero(Points);

            for (int i = 0; i < Points; i++) {
                errors[i] = FittingValue(X[i], parameters) - Y[i];

                approxs[i] = FittingValue(X[i], parameters);
            }

            return errors;
        }

        /// <summary>フィッティング値</summary>
        public abstract MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters);

        /// <summary>フィッティング値</summary>
        public MultiPrecision<N>[] FittingValue(IReadOnlyList<MultiPrecision<N>> xs, Vector<N> parameters) {
            List<MultiPrecision<N>> ys = new();

            for (int i = 0; i < xs.Count; i++) {
                ys.Add(FittingValue(xs[i], parameters));
            }

            return ys.ToArray();
        }
    }
}
