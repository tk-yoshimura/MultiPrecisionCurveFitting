using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    /// <summary>フィッティング基本クラス</summary>
    public abstract class Fitter<N> where N : struct, IConstant {

        /// <summary>フィッティング対象の独立変数</summary>
        public Vector<N> X { get; private set; }

        /// <summary>フィッティング対象の従属変数</summary>
        public Vector<N> Y { get; private set; }

        /// <summary>フィッティング対象数</summary>
        public int Points { get; private set; }

        /// <summary>パラメータ数</summary>
        public int Parameters { get; private set; }

        /// <summary>コンストラクタ</summary>
        public Fitter(Vector<N> xs, Vector<N> ys, int parameters) {
            if (xs.Dim < parameters || xs.Dim != ys.Dim) {
                throw new ArgumentException("mismatch size", $"{nameof(xs)},{nameof(ys)}");
            }
            if (parameters < 1) {
                throw new ArgumentOutOfRangeException(nameof(parameters));
            }

            this.X = xs.Copy();
            this.Y = ys.Copy();
            this.Points = xs.Dim;
            this.Parameters = parameters;
        }

        /// <summary>誤差二乗和</summary>
        public MultiPrecision<N> Cost(Vector<N> parameters) {
            if (parameters.Dim != Parameters) {
                throw new ArgumentException("invalid size", nameof(parameters));
            }

            Vector<N> errors = Error(parameters);
            MultiPrecision<N> cost = errors.SquareNorm;

            return cost;
        }

        /// <summary>誤差</summary>
        public Vector<N> Error(Vector<N> parameters) {
            if (parameters.Dim != Parameters) {
                throw new ArgumentException("invalid size", nameof(parameters));
            }

            Vector<N> errors = FittingValue(X, parameters) - Y;

            return errors;
        }

        /// <summary>フィッティング値</summary>
        public abstract MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters);

        /// <summary>フィッティング値</summary>
        public Vector<N> FittingValue(Vector<N> xs, Vector<N> parameters) {
            MultiPrecision<N>[] ys = new MultiPrecision<N>[xs.Dim];

            for (int i = 0; i < xs.Dim; i++) {
                ys[i] = FittingValue(xs[i], parameters);
            }

            return ys;
        }
    }
}
