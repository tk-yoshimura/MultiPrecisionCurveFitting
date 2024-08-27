using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiPrecision;
using MultiPrecisionAlgebra;

namespace MultiPrecisionCurveFitting.Tests {
    [TestClass()]
    public class GaussNewtonFitterTests {
        [TestMethod()]
        public void FitTest() {
            static MultiPrecision<Pow2.N8> fitting_func(MultiPrecision<Pow2.N8> x, Vector<Pow2.N8> parameter) {
                MultiPrecision<Pow2.N8> a = parameter[0], b = parameter[1];

                return b / (1 + MultiPrecision<Pow2.N8>.Exp((-a) * x));
            }

            static Vector<Pow2.N8> fitting_diff_func(MultiPrecision<Pow2.N8> x, Vector<Pow2.N8> parameter) {
                MultiPrecision<Pow2.N8> a = parameter[0], b = parameter[1];

                MultiPrecision<Pow2.N8> v = MultiPrecision<Pow2.N8>.Exp(-a * x) + 1;

                return new Vector<Pow2.N8>(b * x * MultiPrecision<Pow2.N8>.Exp(-a * x) / (v * v), 1 / v);
            }

            Vector<Pow2.N8> p = new(2, 3);
            MultiPrecision<Pow2.N8>[] xs = [1, 3, 4, 7];
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => fitting_func(x, p), xs);

            GaussNewtonFitter<Pow2.N8> fitter = new(xs, ys, new FittingFunction<Pow2.N8>(2, fitting_func, fitting_diff_func));

            var v = fitter.Fit(new Vector<Pow2.N8>(3, 4), iter: 256);

            Assert.IsTrue((v - p).Norm < 1e-40);
        }

        [TestMethod()]
        public void ExecuteWeightedFittingTest() {
            static MultiPrecision<Pow2.N8> fitting_func(MultiPrecision<Pow2.N8> x, Vector<Pow2.N8> parameter) {
                MultiPrecision<Pow2.N8> a = parameter[0], b = parameter[1];

                return b / (1 + MultiPrecision<Pow2.N8>.Exp((-a) * x));
            }

            static Vector<Pow2.N8> fitting_diff_func(MultiPrecision<Pow2.N8> x, Vector<Pow2.N8> parameter) {
                MultiPrecision<Pow2.N8> a = parameter[0], b = parameter[1];

                MultiPrecision<Pow2.N8> v = MultiPrecision<Pow2.N8>.Exp(-a * x) + 1;

                return new Vector<Pow2.N8>(b * x * MultiPrecision<Pow2.N8>.Exp(-a * x) / (v * v), 1 / v);
            }

            Vector<Pow2.N8> p = new(2, 3);
            MultiPrecision<Pow2.N8>[] xs = [1, 3, 4, 7, 8];
            MultiPrecision<Pow2.N8>[] ys = Vector<Pow2.N8>.Func(x => fitting_func(x, p), xs);
            ys[2] = 1e+8;

            MultiPrecision<Pow2.N8>[] ws = [0.5, 0.75, 0, 0.75, 0.5];

            GaussNewtonFitter<Pow2.N8> fitter = new(xs, ys, new FittingFunction<Pow2.N8>(2, fitting_func, fitting_diff_func));

            var v = fitter.Fit(new Vector<Pow2.N8>(3, 4), iter: 256, weights: ws);

            Assert.IsTrue((v - p).Norm < 1e-40);
        }
    }
}