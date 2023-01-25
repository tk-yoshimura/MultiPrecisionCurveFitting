﻿using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {

    /// <summary>重み付き多項式フィッティング</summary>
    public class WeightedPolynomialFitter<N> : Fitter<N> where N : struct, IConstant {

        private readonly IReadOnlyList<MultiPrecision<N>> weights;

        /// <summary>次数</summary>
        public int Degree {
            get; private set;
        }

        /// <summary>y切片を有効にするか</summary>
        public bool EnableIntercept { get; set; }

        /// <summary>コンストラクタ</summary>
        public WeightedPolynomialFitter(IReadOnlyList<MultiPrecision<N>> xs, IReadOnlyList<MultiPrecision<N>> ys, IReadOnlyList<MultiPrecision<N>> weights, int degree, bool enable_intercept)
            : base(xs, ys, checked(degree + (enable_intercept ? 1 : 0))) {

            this.Degree = degree;
            this.EnableIntercept = enable_intercept;

            if (Points != weights.Count || !weights.All(w => w.Sign == Sign.Plus)) {
                throw new ArgumentException(null, nameof(weights));
            }

            this.weights = weights;
        }

        /// <summary>重み付き誤差二乗和</summary>
        public MultiPrecision<N> WeightedCost(Vector<N> coefficients) {
            if (coefficients is null) {
                throw new ArgumentNullException(nameof(coefficients));
            }
            if (coefficients.Dim != Parameters) {
                throw new ArgumentException(null, nameof(coefficients));
            }

            Vector<N> errors = Error(coefficients);
            MultiPrecision<N> cost = 0;
            for (int i = 0; i < errors.Dim; i++) {
                cost += weights[i] * errors[i] * errors[i];
            }

            return cost;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> coefficients) {
            if (EnableIntercept) {
                MultiPrecision<N> y = coefficients[0], ploy_x = 1;

                for (int i = 1; i < coefficients.Dim; i++) {
                    ploy_x *= x;
                    y += ploy_x * coefficients[i];
                }

                return y;
            }
            else {
                MultiPrecision<N> y = 0, ploy_x = 1;

                for (int i = 0; i < coefficients.Dim; i++) {
                    ploy_x *= x;
                    y += ploy_x * coefficients[i];
                }

                return y;
            }
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting() {
            Matrix<N> m = Matrix<N>.Zero(Points, Parameters);
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

            Matrix<N> m_transpose = m.Transpose;

            for (int i = 0; i < Points; i++) {
                for (int j = 0; j < m_transpose.Rows; j++) {
                    m_transpose[j, i] *= weights[i];
                }
            }

            return (m_transpose * m).Inverse * m_transpose * b;
        }
    }
}
