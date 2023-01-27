﻿using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting {
    /// <summary>Gauss-Newton法</summary>
    public class GaussNewtonFitter<N> : Fitter<N> where N : struct, IConstant {
        private readonly FittingFunction<N> func;

        /// <summary>コンストラクタ</summary>
        public GaussNewtonFitter(Vector<N> xs, Vector<N> ys, FittingFunction<N> func)
            : base(xs, ys, func.Parameters) {

            this.func = func;
        }

        /// <summary>フィッティング値</summary>
        public override MultiPrecision<N> FittingValue(MultiPrecision<N> x, Vector<N> parameters) {
            return func.F(x, parameters);
        }

        /// <summary>フィッティング</summary>
        public Vector<N> ExecuteFitting(Vector<N> parameters, double lambda = 0.75, int iter = 256, Vector<N>? weights = null, Func<Vector<N>, bool>? iter_callback = null) {
            Vector<N> errors, dparam;
            Matrix<N> jacobian;

            for (int j = 0; j < iter; j++) {
                errors = weights is null ? Error(parameters) : weights * Error(parameters);
                jacobian = Jacobian(parameters);
                dparam = jacobian.Inverse * errors;

                if (!Vector<N>.IsValid(dparam)) {
                    break;
                }

                parameters -= dparam * lambda;

                if (iter_callback is not null) {
                    if (!iter_callback(parameters)) {
                        break;
                    }
                }
            }

            return parameters;
        }

        /// <summary>ヤコビアン行列</summary>
        private Matrix<N> Jacobian(Vector<N> parameters) {
            Matrix<N> jacobian = Matrix<N>.Zero(Points, func.Parameters);

            for (int i = 0, j; i < Points; i++) {
                Vector<N> df = func.DiffF(X[i], parameters);

                for (j = 0; j < parameters.Dim; j++) {
                    jacobian[i, j] = df[j];
                }
            }

            return jacobian;
        }
    }
}
